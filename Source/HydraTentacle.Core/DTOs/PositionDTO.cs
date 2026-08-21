using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("Position")]
    public class PositionDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public Guid OrganizationUnitId { get; set; }
        public string OrganizationUnit_Name { get; set; } = string.Empty;
        public Guid OrganizationUnit_Id { get; set; }

        public PositionDTO()
        {
            SetControllerName("Position");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PositionDTO>(x => x.Name),
                displayName: "Pozisyon Adı",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PositionDTO>(x => x.Description),
                displayName: "Açıklama",
                htmlElementTypeInCreationAndEdit: HtmlElementType.TextArea
            );

            SetConfigurationsForNavigations(
                leftTableKeyName: "OrganizationUnitId",
                rightTableName: "OrganizationUnit",
                columnNameToDisplay: "Name",
                displayName: "Birim"
            );

            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
