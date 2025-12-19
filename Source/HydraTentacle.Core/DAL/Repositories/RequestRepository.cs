using Hydra.Core;
using Hydra.DAL.Core;
using Hydra.DI;
using HydraTentacle.Core.Models;

namespace HydraTentacle.Core.DAL.Repositories
{
    [RegisterAsRepository(typeof(IRepository<Request>))]
    public class RequestRepository : Repository<Request>
    {
        public RequestRepository(RepositoryInjector injector) : base(injector)
        {
        }
    }
}
