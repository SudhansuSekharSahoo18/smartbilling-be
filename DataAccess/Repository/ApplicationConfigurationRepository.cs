using Models.Models;
using SmartBillingServer.DataAccess.Data;

namespace DataAccess.Repository
{
    public class ApplicationConfigurationRepository : Repository<ApplicationConfiguration>, IApplicationConfigurationRepository
    {
        private ApplicationDbContext _db;
        public ApplicationConfigurationRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
