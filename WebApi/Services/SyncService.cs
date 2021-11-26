using Common;
using System.Text.Json;
using WebApi.Settings;

namespace WebApi.Services
{
    public class SyncService<T> : ISyncService<T> where T : BaseEntity
    {
        private readonly ISyncServiceSettings _settings;
        private readonly IHttpContextAccessor _accessor;

        public SyncService(ISyncServiceSettings settings, IHttpContextAccessor accessor) => (_settings, _accessor) = (settings, accessor);

        public HttpResponseMessage Delete(T record)
        {
            var syncType = _settings.DeleteHttpMethod;
            var json = ToSyncEntityJson(record, syncType);

            return HttpClientUtils.SendJson(json, _settings.Host, "POST");
        }

        public HttpResponseMessage Upsert(T record)
        {
            var syncType = _settings.UpsertHttpMethod;
            var json = ToSyncEntityJson(record, syncType);

            return HttpClientUtils.SendJson(json, _settings.Host, "POST");
        }

        private string ToSyncEntityJson(T record, string syncType)
        {
            var objectType = typeof(T);

            var syncEntity = new SyncEntity
            {
                JsonData = JsonSerializer.Serialize(record),
                SyncType = syncType,
                ObjectType = objectType.Name,
                Id = record.Id,
                LastModifiedAt = record.LastModifiedAt,
                Origin = _accessor.HttpContext!.Request.Host.ToString()
            };

            var json = JsonSerializer.Serialize(syncEntity);

            return json;
        }
    }
}
