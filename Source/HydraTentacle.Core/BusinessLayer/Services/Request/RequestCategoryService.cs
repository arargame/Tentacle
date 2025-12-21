using Hydra.Services.Core;
using Hydra.DI;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Core.BusinessLayer.Services.Request
{
    [RegisterAsService(typeof(IService<RequestCategory>))]
    public class RequestCategoryService : Service<RequestCategory>
    {
        public RequestCategoryService(ServiceInjector injector) : base(injector)
        {
        }
    }
}
