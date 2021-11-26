namespace Common
{
    public class SyncEntity
    {
        public int Id { get; set; }
        public DateTime LastModifiedAt { get; set; } 
        public string JsonData { get; set; } = null!;
        public string SyncType { get; set; } = null!;
        public string ObjectType { get; set; } = null!;
        public string Origin { get; set; } = null!;

    }
}
