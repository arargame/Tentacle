using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("RequestCategory")]
    public class RequestCategoryDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public bool IsAssignable { get; set; }

        public RequestCategoryDTO()
        {
            SetControllerName("RequestCategory");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RequestCategoryDTO>(x => x.Name),
                displayName: "Kategori Adı",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RequestCategoryDTO>(x => x.Description),
                displayName: "Açıklama",
                htmlElementTypeInCreationAndEdit: HtmlElementType.TextArea
            );

            SetConfigurationsViaBooleanPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RequestCategoryDTO>(x => x.IsAssignable),
                displayName: "Atanabilir"
            );

            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
