using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;
using System.Collections.Generic;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("Employee")]
    public class EmployeeDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public string? Surname { get; set; }
        public bool IsActiveEmployee { get; set; } = true;

        public Guid PositionId { get; set; }
        public string Position_Name { get; set; } = string.Empty;
        public Guid Position_Id { get; set; }

        public EmployeeDTO()
        {
            SetControllerName("Employee");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            // 1. First Name (Ad) - span 6 (Row 1, Column 1)
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<EmployeeDTO>(x => x.Name),
                displayName: "First Name",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            // 2. Surname (Soyad) - span 6 (Row 1, Column 2)
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<EmployeeDTO>(x => x.Surname),
                displayName: "Surname",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            // 3. Position Navigation - span 6 (Row 2, Column 1)
            SetConfigurationsForNavigations(
                leftTableKeyName: "PositionId",
                rightTableName: "Position",
                columnNameToDisplay: "Name",
                displayName: "Position"
            );

            // 4. IsActiveEmployee - span 6 (Row 2, Column 2)
            SetConfigurationsViaPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<EmployeeDTO>(x => x.IsActiveEmployee),
                configurations: new List<IConfiguration>
                {
                    new CreateViewConfiguration(elementType: HtmlElementType.Input, inputType: HtmlInputType.checkbox, defaultValue: true),
                    new EditViewConfiguration(elementType: HtmlElementType.Input, inputType: HtmlInputType.checkbox),
                    new ListViewConfiguration(toFilter: new AttributeToFilter(nameof(EqualFilter)), toOrder: new AttributeToOrder(isOrderable: true), isVisibleInListView: true),
                    new DetailsViewConfiguration()
                },
                displayName: "Active"
            );

            // 5. Description - span 12 (Row 3, Wide TextArea)
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<EmployeeDTO>(x => x.Description),
                displayName: "Description",
                htmlElementTypeInCreationAndEdit: HtmlElementType.TextArea
            );

            SetConfigurationsForBaseObjectMembers(o => o.IsActive, o => o.Description);

            return this;
        }
    }
}
