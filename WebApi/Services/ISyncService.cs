using Common;

namespace WebApi.Services
{
    public interface ISyncService<T> where T : BaseEntity
    {
        HttpResponseMessage Upsert(T record);
        HttpResponseMessage Delete(T record);
    }
}
