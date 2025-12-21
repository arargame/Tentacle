using System;
using Hydra.Core.HumanResources;

namespace HydraTentacle.Core.Models.Request
{
    public class RequestCategoryResponsiblePosition
    {
        public Guid RequestCategoryId { get; set; }
        public RequestCategory RequestCategory { get; set; } = null!;

        public Guid PositionId { get; set; }
        public Position Position { get; set; } = null!;
    }
}
