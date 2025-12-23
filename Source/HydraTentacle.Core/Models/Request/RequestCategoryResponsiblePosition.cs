using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Hydra.Core;
using Hydra.Core.HumanResources;

namespace HydraTentacle.Core.Models.Request
{
    public class RequestCategoryResponsiblePosition : BaseObject<RequestCategoryResponsiblePosition>
    {
        [Required]
        public Guid RequestCategoryId { get; set; }
        
        [ForeignKey("RequestCategoryId")]
        public RequestCategory RequestCategory { get; set; } = null!;

        [Required]
        public Guid PositionId { get; set; }
        
        [ForeignKey("PositionId")]
        public Position Position { get; set; } = null!;
    }
}
