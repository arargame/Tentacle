using Hydra.Services.Core;
using Hydra.DI;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Core.BusinessLayer.Services.Request
{
    [RegisterAsService(typeof(IService<HydraTentacle.Core.Models.Request.Request>))]
    public class RequestService : Service<HydraTentacle.Core.Models.Request.Request>
    {
        public RequestService(ServiceInjector injector) : base(injector)
        {
        }
    }
}
