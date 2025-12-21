using System;
using Hydra.Core;
using Hydra.Core.HumanResources;

namespace HydraTentacle.Core.Models.Request
{
    public class RequestAssignment : BaseObject<RequestAssignment>
    {
        public Guid RequestId { get; set; }
        public Request Request { get; set; } = null!;

        public Guid AssignedEmployeeId { get; set; }
        public Employee AssignedEmployee { get; set; } = null!;

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    }
}
