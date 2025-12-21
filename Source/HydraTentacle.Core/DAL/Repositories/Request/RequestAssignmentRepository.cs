using Hydra.DAL.Core;
using Hydra.DI;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Core.DAL.Repositories.Request
{
    [RegisterAsRepository(typeof(IRepository<RequestAssignment>))]
    public class RequestAssignmentRepository : Repository<RequestAssignment>
    {
        public RequestAssignmentRepository(RepositoryInjector injector) : base(injector)
        {
        }
    }
}
