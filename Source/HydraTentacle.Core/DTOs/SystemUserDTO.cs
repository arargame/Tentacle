using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;
using System.Collections.Generic;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("SystemUser")]
    public class SystemUserDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public string? Email { get; set; } = null;
        public bool EmailConfirmed { get; set; }

        public SystemUserDTO()
        {
            SetControllerName("SystemUser");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            // 1. Email
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<SystemUserDTO>(x => x.Email!),
                displayName: "Email",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            // 2. EmailConfirmed (ListView, CollectionView, DetailsView, EditView) - CreateView YOK
            SetConfigurationsViaPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<SystemUserDTO>(x => x.EmailConfirmed),
                configurations: new List<IConfiguration>
                {
                    new ListViewConfiguration(
                        toFilter: new AttributeToFilter(nameof(EqualFilter)),
                        toOrder: new AttributeToOrder(isOrderable: true),
                        elementType: HtmlElementType.DropdownList)
                        .AlsoUseToCreateCollectionViewConfiguration(),

                    new EditViewConfiguration(elementType: HtmlElementType.DropdownList),

                    new DetailsViewConfiguration()
                },
                displayName: "Email Confirmed"
            );

            // 3. IsActive (ListView, CollectionView, DetailsView, EditView) - CreateView YOK
            SetConfigurationsViaPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<SystemUserDTO>(x => x.IsActive),
                configurations: new List<IConfiguration>
                {
                    new ListViewConfiguration(
                        toFilter: new AttributeToFilter(nameof(EqualFilter)),
                        toOrder: new AttributeToOrder(isOrderable: true),
                        elementType: HtmlElementType.DropdownList)
                        .AlsoUseToCreateCollectionViewConfiguration(),

                    new EditViewConfiguration(elementType: HtmlElementType.DropdownList),

                    new DetailsViewConfiguration()
                },
                displayName: "Active"
            );

            // 4. AddedDate
            SetConfigurationsViaPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<SystemUserDTO>(x => x.AddedDate),
                configurations: new List<IConfiguration>
                {
                    new ListViewConfiguration(
                        toFilter: new AttributeToFilter(nameof(BetweenFilter)),
                        toOrder: new AttributeToOrder(isOrderable: true))
                        .AlsoUseToCreateCollectionViewConfiguration(),

                    new DetailsViewConfiguration()
                },
                displayName: "Added Date"
            );

            // 5. ModifiedDate
            SetConfigurationsViaPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<SystemUserDTO>(x => x.ModifiedDate),
                configurations: new List<IConfiguration>
                {
                    new ListViewConfiguration(
                        toFilter: new AttributeToFilter(nameof(BetweenFilter)),
                        toOrder: new AttributeToOrder(isOrderable: true)),

                    new DetailsViewConfiguration()
                },
                displayName: "Modified Date"
            );

            SortByWrittenOrder();

            return this;
        }
    }
}
