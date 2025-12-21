using Hydra.DI;
using HydraTentacle.Core.DTOs;
using HydraTentacle.Core.DTOs;
using RequestModel = HydraTentacle.Core.Models.Request.Request;
using Hydra.Services.Core;
using Microsoft.AspNetCore.Mvc;

namespace HydraTentacle.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : MainController<RequestModel>
    {
        private readonly IServiceFactory _serviceFactory;

        public RequestController(IControllerInjector injector) : base(injector)
        {
            _serviceFactory = injector.ServiceFactory;
        }

#if DEBUG
        [HttpPost("Seed/{count:int}")]
        public override async Task<IActionResult> Seed(int count)
        {
            // 1. Get or Create an Employee for FK
            var employeeService = _serviceFactory.GetService<Hydra.Services.Core.IService<Hydra.Core.HumanResources.Employee>>();
            var employee = (await employeeService.SelectWithLinqAsync(countToTake: 1)).FirstOrDefault();

            if (employee == null)
            {
                // Create OrganizationUnit first
                var orgService = _serviceFactory.GetService<Hydra.Services.Core.IService<Hydra.Core.HumanResources.OrganizationUnit>>();
                var orgUnit = (await orgService.SelectWithLinqAsync(countToTake: 1)).FirstOrDefault();
                if (orgUnit == null)
                {
                    orgUnit = new Hydra.Core.HumanResources.OrganizationUnit { Name = "Seed Org", Description = "Auto Generated" };
                    await orgService.CreateAsync(orgUnit);
                }

                // Create Position
                var positionService = _serviceFactory.GetService<Hydra.Services.Core.IService<Hydra.Core.HumanResources.Position>>();
                var position = (await positionService.SelectWithLinqAsync(countToTake: 1)).FirstOrDefault();
                
                if (position == null)
                {
                    position = new Hydra.Core.HumanResources.Position { Name = "Seed Position", Description = "Auto Generated", OrganizationUnitId = orgUnit.Id };
                    await positionService.CreateAsync(position);
                }

                employee = new Hydra.Core.HumanResources.Employee
                {
                    Name = "Seed Employee",
                    Description = "Auto Generated",
                    PositionId = position.Id,
                    IsActiveEmployee = true
                };
                await employeeService.CreateAsync(employee);
            }


            // 2. Generate Requests with valid FK
            for (int i = 0; i < count; i++)
            {
                var request = Hydra.TestManagement.SampleDataFactory.CreateSample<RequestModel>();
                request.CreatedByEmployeeId = employee.Id; // Fix FK
                
                // Ensure other FKs if necessary (RequestCategory)
                var catService = _serviceFactory.GetService<Hydra.Services.Core.IService<HydraTentacle.Core.Models.Request.RequestCategory>>();
                var cat = (await catService.SelectWithLinqAsync(countToTake:1)).FirstOrDefault();
                if(cat == null)
                {
                     cat = new HydraTentacle.Core.Models.Request.RequestCategory { Name = "Seed Category", Description = "Auto" };
                     await catService.CreateAsync(cat);
                }
                request.RequestCategoryId = cat.Id;

                await Service.CreateAsync(request);
            }

            return Ok($"{count} requests generated with EmployeeId: {employee.Id}");
        }
#endif
    }
}
