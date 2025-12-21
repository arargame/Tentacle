using Hydra.DAL.Core;
using Hydra.DI;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Core.DAL.Repositories.Request
{
    [RegisterAsRepository(typeof(IRepository<RequestAttachment>))]
    public class RequestAttachmentRepository : Repository<RequestAttachment>
    {
        public RequestAttachmentRepository(RepositoryInjector injector) : base(injector)
        {
        }
    }
}
