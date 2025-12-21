using System;
using System.Collections.Generic;
using Hydra.Core;
using Hydra.Core.HumanResources;
using HydraTentacle.Core.Constants;

namespace HydraTentacle.Core.Models.Request
{
    public class Request : BaseObject<Request>
    {
        public Guid RequestCategoryId { get; set; }
        public RequestCategory RequestCategory { get; set; } = null!;

        public Guid CreatedByEmployeeId { get; set; }
        public Employee CreatedByEmployee { get; set; } = null!;

        public RequestStatus Status { get; set; } = RequestStatus.Open;
        public RequestPriority Priority { get; set; } = RequestPriority.Normal;

        public DateTime? DueDate { get; set; }

        public List<RequestAssignment> Assignments { get; set; } = new();
        public List<RequestAttachment> Attachments { get; set; } = new();
    }
}
