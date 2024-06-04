using SmartBillingServer.DataAccess.Data;
using SmartBillingServer.Models;

namespace DataAccess.Repository
{
    public class BillRepository : Repository<Bill>, IBillRepository
    {
        private ApplicationDbContext _db;
        public BillRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
