using Hydra.DI;
using HydraTentacle.Core.DTOs;
using HydraTentacle.Core.DTOs;
using RequestModel = HydraTentacle.Core.Models.Request.Request;
using Microsoft.AspNetCore.Mvc;

namespace HydraTentacle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : MainController<RequestModel>
    {
        public RequestController(IControllerInjector injector) : base(injector)
        {
        }
    }
}
