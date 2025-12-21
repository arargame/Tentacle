using System;
using System.Collections.Generic;
using Hydra.Core;

namespace HydraTentacle.Core.Models.Request
{
    public class RequestCategory : BaseObject<RequestCategory>
    {
        public bool IsAssignable { get; set; } = true;

        // Positions responsible for this category
        public List<RequestCategoryResponsiblePosition> ResponsiblePositions { get; set; } = new();
    }
}
