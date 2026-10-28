using SmartBillingServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repository
{
    public interface IBillRepository: IRepository<Bill>
    {
        // Saves changes made to entities already loaded through this repository (e.g. a bill's items).
        void Save();
    }
}
