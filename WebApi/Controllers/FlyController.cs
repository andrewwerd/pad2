using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.DataAccess;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/fly")]
    public class FlyController : ControllerBase
    {

        private readonly ISyncService<Fly> _flySyncService;
        private readonly ApplicationDbContext _context;

        public FlyController(ISyncService<Fly> flySyncService, ApplicationDbContext context) => (_flySyncService, _context) = (flySyncService, context);

        [HttpGet]
        public async Task<IEnumerable<Fly>> Get(CancellationToken cancellationToken = default)
        {

            return await _context.Flies.ToListAsync(cancellationToken);
        }

        [HttpGet("{id}")]
        public async Task<Fly> GetById(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Flies.FirstOrDefaultAsync(fly => fly.Id == id, cancellationToken);

        }

        [HttpPost]
        public async Task<Fly> Create([FromBody] Fly fly, CancellationToken cancellationToken = default)
        {
            _context.Flies.Add(fly);
            await _context.SaveChangesAsync(cancellationToken);

            _flySyncService.Upsert(fly);
            return fly;

        }

        [HttpPut]
        public async Task<Fly> Update([FromBody] Fly fly, CancellationToken cancellationToken = default)
        {
            _context.Flies.Update(fly);
            fly.LastModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            _flySyncService.Upsert(fly);
            return fly;

        }

        [HttpPut("sync")]
        public async Task<Fly> UpsertSync(Fly fly, CancellationToken cancellationToken = default)
        {
            var existingFly = await _context.Flies.FirstOrDefaultAsync(cachedFly => cachedFly.Id == fly.Id, cancellationToken);
            if (existingFly == null)
            {
                _context.Add(fly);
            }
            else if (fly.LastModifiedAt > existingFly.LastModifiedAt)
            {
                fly.LastModifiedAt = DateTime.UtcNow;
                _context.Flies.Update(fly);
            }

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Flies] ON;", cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT [dbo].[Flies] OFF", cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return fly;

        }

        [HttpDelete("sync")]
        public async Task<Fly> DeleteSync(Fly fly, CancellationToken cancellationToken = default)
        {
            var existingFly = await _context.Flies.FirstOrDefaultAsync(cachedFly => cachedFly.Id == fly.Id, cancellationToken);
            if (existingFly != null && fly.LastModifiedAt > existingFly.LastModifiedAt)
            {
                _context.Remove(existingFly);
            }

            await _context.SaveChangesAsync(cancellationToken);

            return fly;

        }

        [HttpDelete("{id}")]
        public async Task<int> Delete(int id, CancellationToken cancellationToken = default)
        {
            var fly = await _context.Flies.FirstOrDefaultAsync(fly => fly.Id == id, cancellationToken);
            _context.Flies.Remove(fly);
            await _context.SaveChangesAsync(cancellationToken);

            fly.LastModifiedAt = DateTime.UtcNow;
            _flySyncService.Upsert(fly);

            return id;
        }
    }
}