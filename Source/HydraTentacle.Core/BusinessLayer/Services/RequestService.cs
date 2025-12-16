using Hydra.Services.Core;
using HydraTentacle.Core.Models;
using Hydra.DI;
using Hydra.Services;

namespace HydraTentacle.Core.BusinessLayer.Services
{
    [RegisterAsService(typeof(IService<Request>))]
    public class RequestService : Service<Request>
    {
        public RequestService(ServiceInjector injector) : base(injector)
        {
        }
    }
}
