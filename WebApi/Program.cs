using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApi.DataAccess;
using WebApi.Services;
using WebApi.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(
    options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISyncService<Fly>, SyncService<Fly>>();

builder.Services.Configure<SyncServiceSettings>(builder.Configuration.GetSection("SyncServiceSettings"));
builder.Services.AddSingleton<ISyncServiceSettings>(provider => provider.GetRequiredService<IOptions<SyncServiceSettings>>().Value);

var app = builder.Build();

var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
db.Database.Migrate();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
