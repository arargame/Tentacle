using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("RequestCategoryResponsiblePosition")]
    public class RequestCategoryResponsiblePositionDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public Guid RequestCategoryId { get; set; }
        public string RequestCategory_Name { get; set; } = string.Empty;
        public Guid RequestCategory_Id { get; set; }

        public Guid PositionId { get; set; }
        public string Position_Name { get; set; } = string.Empty;
        public Guid Position_Id { get; set; }

        public RequestCategoryResponsiblePositionDTO()
        {
            SetControllerName("RequestCategoryResponsiblePosition");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsForNavigations(
                leftTableKeyName: "RequestCategoryId",
                rightTableName: "RequestCategory",
                columnNameToDisplay: "Name",
                displayName: "Kategori"
            );

            SetConfigurationsForNavigations(
                leftTableKeyName: "PositionId",
                rightTableName: "Position",
                columnNameToDisplay: "Name",
                displayName: "Pozisyon"
            );

            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
