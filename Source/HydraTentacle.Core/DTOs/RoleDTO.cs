using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("Role")]
    public class RoleDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public RoleDTO()
        {
            SetControllerName("Role");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RoleDTO>(x => x.Name),
                displayName: "Rol Adı",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RoleDTO>(x => x.Description),
                displayName: "Açıklama",
                htmlElementTypeInCreationAndEdit: HtmlElementType.TextArea
            );

            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
