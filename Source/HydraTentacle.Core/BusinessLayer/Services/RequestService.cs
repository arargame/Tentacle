using Hydra.Services.Core;
using HydraTentacle.Core.Models;
using Hydra.DI;
using Hydra.Services;
using System.Threading.Tasks;
using System.Linq;
using Hydra.TestManagement;

namespace HydraTentacle.Core.BusinessLayer.Services
{
    [RegisterAsService(typeof(IService<Request>))]
    public class RequestService : Service<Request>
    {
        public RequestService(ServiceInjector injector) : base(injector)
        {
        }

        public override async Task<int> SeedAsync(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var request = SampleDataFactory.CreateSample<Request>();
                await CreateAsync(request);
            }
            return count;
        }
    }
}
