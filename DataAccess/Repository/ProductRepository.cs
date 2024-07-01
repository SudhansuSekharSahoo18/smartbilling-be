using Models.Models;
using SmartBillingServer.DataAccess.Data;

namespace DataAccess.Repository
{
    public class ProductRepository : Repository<Item>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
