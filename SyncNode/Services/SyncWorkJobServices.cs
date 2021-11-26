using Common;
using SyncNode.Settings;
using System.Collections.Concurrent;

namespace SyncNode.Services
{
    public class SyncWorkJobServices : IHostedService
    {
        private readonly ConcurrentDictionary<int, SyncEntity> entities = new ConcurrentDictionary<int, SyncEntity>();
        private readonly IApiSettings _apiSettings;

        private Timer _timer = null!;

        public SyncWorkJobServices(IApiSettings apiSettings) => _apiSettings = apiSettings;
        public void AddItem(SyncEntity entity)
        {
            bool exists = entities.TryGetValue(entity.Id, out var item);

            if (!exists || (exists && entity.LastModifiedAt > item!.LastModifiedAt))
            {
                entities[entity.Id] = entity;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(sendCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        private void sendCallback(object? state)
        {
            foreach (var entity in entities)
            {
                var exists = entities.TryRemove(entity.Key, out var item);

                if (exists)
                {
                    var receviers = _apiSettings.Hosts.Where(x => !x.Contains(item!.Origin));

                    foreach (var recevier in receviers)
                    {
                        var url = $"{recevier}/{item!.ObjectType}/sync";

                        try
                        {
                            var result = HttpClientUtils.SendJson(item!.JsonData, url, item!.SyncType);

                        }
                        catch (Exception ex)
                        {

                        }
                    }
                }
            }
        }
    }
}
