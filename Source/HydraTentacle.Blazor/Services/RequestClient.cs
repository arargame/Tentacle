using Hydra.RazorClassLibrary.Services.Core;
using HydraTentacle.Core.Models.Request;

namespace HydraTentacle.Blazor.Services
{
    public class RequestClient : ApiClient<Request>
    {
        public RequestClient(Hydra.RazorClassLibrary.Services.Http.IHttpClientService httpClientService) : base(httpClientService)
        {
        }
    }
}
