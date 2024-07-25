using Models.Models;
using SmartBillingServer.DataAccess.Data;

namespace DataAccess.Repository
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        private ApplicationDbContext _db;
        public ItemRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
