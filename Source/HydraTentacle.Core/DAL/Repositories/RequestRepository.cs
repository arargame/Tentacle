using Hydra.DAL.Core;
using HydraTentacle.Core.Models;
using Hydra.DI;

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
