using Hydra.DAL.Core;
using Hydra.DI;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Core.DAL.Repositories.Request
{
    [RegisterAsRepository(typeof(IRepository<RequestCategory>))]
    public class RequestCategoryRepository : Repository<RequestCategory>
    {
        public RequestCategoryRepository(RepositoryInjector injector) : base(injector)
        {
        }
    }
}
