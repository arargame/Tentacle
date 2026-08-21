using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;
using HydraTentacle.Core.Constants;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("Request")]
    public class RequestDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        // Name and Description are inherited from ViewDTO
        public RequestStatus Status { get; set; }
        public RequestPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }

        // Category navigation (property öneki == tablo adı olduğu için otomatik yapılandırılır)
        public Guid RequestCategoryId { get; set; }
        public string RequestCategory_Name { get; set; } = string.Empty;
        public Guid RequestCategory_Id { get; set; }

        // Employee'ye İKİ ayrı join: alias mekanizması ile ayrışırlar.
        // Flatten property önekleri join alias'ından gelir.
        public Guid CreatedByEmployeeId { get; set; }
        public string CreatedByEmployee_Name { get; set; } = string.Empty;
        public Guid CreatedByEmployee_Id { get; set; }

        public Guid? OwnerEmployeeId { get; set; }
        public string OwnerEmployee_Name { get; set; } = string.Empty;
        public Guid OwnerEmployee_Id { get; set; }

        public RequestDTO()
        {
            SetControllerName("Request");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            // Name
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.Name),
                displayName: "Talep Başlığı",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            // Description
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.Description),
                displayName: "Açıklama",
                htmlElementTypeInCreationAndEdit: HtmlElementType.TextArea
            );

            // Status
            SetConfigurationsViaEnumPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.Status),
                displayName: "Durum"
            );

            // Priority
            SetConfigurationsViaEnumPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.Priority),
                displayName: "Öncelik"
            );

            // DueDate
            SetConfigurationsViaPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.DueDate),
                configurations: new List<IConfiguration>
                {
                    new CreateViewConfiguration(elementType: HtmlElementType.Input, inputType: HtmlInputType.date),
                    new EditViewConfiguration(elementType: HtmlElementType.Input, inputType: HtmlInputType.date),
                    new ListViewConfiguration(toFilter: new AttributeToFilter(nameof(BetweenFilter)),
                                              toOrder: new AttributeToOrder(isOrderable: true)),
                    new DetailsViewConfiguration()
                },
                displayName: "Termin Tarihi");

            // Category Navigation (ListView/Details join kolonları + CreateView/Edit FK dropdown)
            SetConfigurationsForNavigations(
                leftTableKeyName: "RequestCategoryId",
                rightTableName: "RequestCategory",
                columnNameToDisplay: "Name",
                displayName: "Kategori"
            );

            // CreatedByEmployee — Employee tablosuna alias'lı join.
            // FK property'si "{alias}Id" = CreatedByEmployeeId konvansiyonuyla otomatik bulunur.
            SetConfigurationsForNavigations(
                leftTableKeyName: "CreatedByEmployeeId",
                rightTableName: "Employee",
                rightTableAlias: "CreatedByEmployee",
                rightTableKeyName: "Id",
                columnNameToDisplay: "Name",
                displayName: "Talep Eden",
                leftTableName: "Request"
            );

            // OwnerEmployee — aynı Employee tablosuna İKİNCİ join (alias sayesinde çakışmaz).
            SetConfigurationsForNavigations(
                leftTableKeyName: "OwnerEmployeeId",
                rightTableName: "Employee",
                rightTableAlias: "OwnerEmployee",
                rightTableKeyName: "Id",
                columnNameToDisplay: "Name",
                displayName: "Sorumlu",
                leftTableName: "Request"
            );

            // Base Object Members (AddedDate, ModifiedDate, IsActive, ...)
            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
