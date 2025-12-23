using System;
using System.Collections.Generic;
using Hydra.Core;
using Hydra.Core.HumanResources;
using HydraTentacle.Core.Constants;

using System.ComponentModel.DataAnnotations.Schema;

namespace HydraTentacle.Core.Models.Request
{
    public class Request : BaseObject<Request>
    {
        public Guid RequestCategoryId { get; set; }
        
        [ForeignKey("RequestCategoryId")]
        public RequestCategory RequestCategory { get; set; } = null!;

        public Guid CreatedByEmployeeId { get; set; }
        
        [ForeignKey("CreatedByEmployeeId")]
        public Employee CreatedByEmployee { get; set; } = null!;

        // Owner/Assignee
        public Guid? OwnerEmployeeId { get; set; }
        
        [ForeignKey("OwnerEmployeeId")]
        public Employee? OwnerEmployee { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Open;
        public RequestPriority Priority { get; set; } = RequestPriority.Normal;

        public DateTime? DueDate { get; set; }

        public List<RequestAttachment> Attachments { get; set; } = new();
    }
}
