using Hydra.DI;
using HydraTentacle.Core.DTOs;
using HydraTentacle.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace HydraTentacle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : MainController<Request>
    {
        public RequestController(IControllerInjector injector) : base(injector)
        {
        }
    }
}
