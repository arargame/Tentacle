using Hydra.Core;

namespace HydraTentacle.Core.DTOs
{
    public class RequestDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        // Name and Description are inherited from BaseObject/ViewDTO
        public HydraTentacle.Core.Constants.RequestStatus Status { get; set; }
        public HydraTentacle.Core.Constants.RequestPriority Priority { get; set; }
        
        // Navigation Properties for flattened view
        public string RequestCategory_Name { get; set; } = string.Empty;
        public string CreatedByEmployee_Name { get; set; } = string.Empty;

        public Guid RequestCategoryId { get; set; }
        public Guid CreatedByEmployeeId { get; set; }

        public Guid RequestCategory_Id { get; set; }
        public Guid CreatedByEmployee_Id { get; set; }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            // Base Object Members (Id, AddedDate, ModifiedDate, IsActive, etc.)
            SetConfigurationsForBaseObjectMembers();

            // Name
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: Hydra.Utils.ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.Name),
                displayName: "Talep Başlığı",
                attributeToFilter: new Hydra.DTOs.ViewConfigurations.AttributeToFilter(nameof(Hydra.DataModels.Filter.ContainsFilter))
            );

            // Description
             SetConfigurationsViaStringPropertyInfo(
                propertyInfo: Hydra.Utils.ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.Description),
                displayName: "Açıklama"
            );

            // Status
            SetConfigurationsViaEnumPropertyInfo(
                propertyInfo: Hydra.Utils.ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.Status),
                displayName: "Durum"
            );

            // Priority
            SetConfigurationsViaEnumPropertyInfo(
                propertyInfo: Hydra.Utils.ReflectionHelper.GetPropertyOf<RequestDTO>(x => x.Priority),
                displayName: "Öncelik"
            );

            // Category Navigation
            SetConfigurationsForNavigations(
                leftTableKeyName: "RequestCategoryId",
                rightTableName: "RequestCategory",
                columnNameToDisplay: "Name",
                displayName: "Kategori"
            );

            // Employee Navigation
            SetConfigurationsForNavigations(
                leftTableKeyName: "CreatedByEmployeeId",
                rightTableName: "CreatedByEmployee", // Table name for Employee might need verification if it differs from property name
                rightTableKeyName: "Id",
                columnNameToDisplay: "Name",
                displayName: "Talep Eden",
                leftTableName: "Request" // Explicitly set left table name if needed
            );

            return this;
        }
    }
}
