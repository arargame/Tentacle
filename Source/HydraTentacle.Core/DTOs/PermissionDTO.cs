using Hydra.DataModels;
using Hydra.DataModels.Filter;
using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.IdentityAndAccess;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    [RegisterAsViewDTO("Permission")]
    public class PermissionDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public PermissionType Type { get; set; }
        public string? Controller { get; set; }
        public string? Action { get; set; }
        public string? Entity { get; set; }
        public bool AllowAnonymous { get; set; }
        public bool Enabled { get; set; }

        public PermissionDTO()
        {
            SetControllerName("Permission");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PermissionDTO>(x => x.Name),
                displayName: "İzin Adı",
                attributeToFilter: new AttributeToFilter(nameof(ContainsFilter))
            );

            SetConfigurationsViaEnumPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PermissionDTO>(x => x.Type),
                displayName: "İzin Tipi"
            );

            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PermissionDTO>(x => x.Controller),
                displayName: "Controller"
            );

            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PermissionDTO>(x => x.Action),
                displayName: "Action"
            );

            SetConfigurationsViaStringPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PermissionDTO>(x => x.Entity),
                displayName: "Entity"
            );

            SetConfigurationsViaBooleanPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PermissionDTO>(x => x.AllowAnonymous),
                displayName: "Anonim Erişim"
            );

            SetConfigurationsViaBooleanPropertyInfo(
                propertyInfo: ReflectionHelper.GetPropertyOf<PermissionDTO>(x => x.Enabled),
                displayName: "Etkin"
            );

            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
