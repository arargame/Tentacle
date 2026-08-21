using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("Employee")]
    public class EmployeeDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public bool IsActiveEmployee { get; set; }

        public Guid PositionId { get; set; }
        public string Position_Name { get; set; } = string.Empty;
        public Guid Position_Id { get; set; }

        public EmployeeDTO()
        {
            SetControllerName("Employee");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<EmployeeDTO>(x => x.Name),
                displayName: "Ad Soyad",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<EmployeeDTO>(x => x.Description),
                displayName: "Açıklama",
                htmlElementTypeInCreationAndEdit: HtmlElementType.TextArea
            );

            SetConfigurationsViaBooleanPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<EmployeeDTO>(x => x.IsActiveEmployee),
                displayName: "Aktif Çalışan"
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
