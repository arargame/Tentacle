using HydraTentacle.Core.Models.Request;
using Hydra.DI;
using Microsoft.AspNetCore.Mvc;
using Hydra.WebApi.Controllers;

namespace HydraTentacle.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequestCategoryResponsiblePositionController : MainController<RequestCategoryResponsiblePosition>
    {
        public RequestCategoryResponsiblePositionController(IControllerInjector controllerInjector) : base(controllerInjector)
        {
        }
    }
}
