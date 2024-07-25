using SmartBillingServer.DataAccess.Data;
using SmartBillingServer.Models;

namespace DataAccess.Repository
{
    public class BarcodeRepository : Repository<Barcode>, IBarcodeRepository
    {
        private ApplicationDbContext _db;
        public BarcodeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
