using Microsoft.AspNetCore.Mvc;
using HydraTentacle.Core.Models;


namespace HydraTentacle.WebApi.Controllers
{
    [ApiController] 
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {


        [HttpGet]
        [Route("GetTestRequests")]
        public IEnumerable<Request> GetTestRequests()
        {
            return new List<Request>
            {
                new Request().SetName("Request1"),
                new Request().SetName("Request2"),
                new Request().SetName("reeuqsat3")
            };
        }


        [HttpGet]
        [Route("GetHello")]
        public IActionResult GetHello()
        {
            return Ok(new { Message = "Hello from RequestController" });
        }
    }
}
