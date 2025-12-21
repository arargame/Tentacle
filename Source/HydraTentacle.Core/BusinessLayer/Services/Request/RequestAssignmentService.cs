using Hydra.Services.Core;
using Hydra.DI;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Core.BusinessLayer.Services.Request
{
    [RegisterAsService(typeof(IService<RequestAssignment>))]
    public class RequestAssignmentService : Service<RequestAssignment>
    {
        public RequestAssignmentService(ServiceInjector injector) : base(injector)
        {
        }
    }
}
