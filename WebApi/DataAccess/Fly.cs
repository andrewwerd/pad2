using Common;

namespace WebApi.DataAccess
{
    public class Fly: BaseEntity
    {

        public string Direction { get; set; } = null!;
        public string Departure { get; set; } = null!;
    }
}
