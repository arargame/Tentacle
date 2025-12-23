using HydraTentacle.Core.Models.Request;
using Hydra.DI;
using Microsoft.AspNetCore.Mvc;
using Hydra.WebApi.Controllers;

namespace HydraTentacle.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestCategoryController : MainController<RequestCategory>
    {
        public RequestCategoryController(IControllerInjector controllerInjector) : base(controllerInjector)
        {
        }
    }
}
