using Models.Models;
using SmartBillingServer.DataAccess.Data;

namespace DataAccess.Repository
{
    public class LabelDesignRepository : Repository<LabelDesign>, ILabelDesignRepository
    {
        private ApplicationDbContext _db;
        public LabelDesignRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
