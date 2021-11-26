namespace WebApi.Settings
{
    public class SyncServiceSettings : ISyncServiceSettings
    {
        public string Host { get; set; } = null!;
        public string UpsertHttpMethod { get; set; } = null!;
        public string DeleteHttpMethod { get; set; } = null!;
    }
}
