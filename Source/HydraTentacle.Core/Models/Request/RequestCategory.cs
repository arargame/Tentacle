using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Hydra.Core;
using Hydra.ValidationManagement;

namespace HydraTentacle.Core.Models.Request
{
    public class RequestCategory : BaseObject<RequestCategory>
    {
        public bool IsAssignable { get; set; } = true;

        // Positions responsible for this category
        public List<RequestCategoryResponsiblePosition> ResponsiblePositions { get; set; } = new();

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            foreach (var result in base.Validate(validationContext))
                yield return result;

            if (string.IsNullOrWhiteSpace(Name))
                yield return ValidationManager.Required(nameof(Name), "Category Name");
        }
    }
}
