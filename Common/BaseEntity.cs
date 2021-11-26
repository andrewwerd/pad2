namespace Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime LastModifiedAt { get; set; }
    }
}
