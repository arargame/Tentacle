using Hydra.Services.Core;
using Hydra.DI;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Core.BusinessLayer.Services.Request
{
    [RegisterAsService(typeof(IService<RequestAttachment>))]
    public class RequestAttachmentService : Service<RequestAttachment>
    {
        public RequestAttachmentService(ServiceInjector injector) : base(injector)
        {
        }
    }
}
