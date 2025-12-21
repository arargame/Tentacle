using Hydra.DAL.Core;
using Hydra.DI;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Core.DAL.Repositories.Request
{
    [RegisterAsRepository(typeof(IRepository<HydraTentacle.Core.Models.Request.Request>))]
    public class RequestRepository : Repository<HydraTentacle.Core.Models.Request.Request>
    {
        public RequestRepository(RepositoryInjector injector) : base(injector)
        {
        }
    }
}
