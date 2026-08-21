using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("OrganizationUnit")]
    public class OrganizationUnitDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public OrganizationUnitDTO()
        {
            SetControllerName("OrganizationUnit");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<OrganizationUnitDTO>(x => x.Name),
                displayName: "Birim Adı",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<OrganizationUnitDTO>(x => x.Description),
                displayName: "Açıklama",
                htmlElementTypeInCreationAndEdit: HtmlElementType.TextArea
            );

            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
