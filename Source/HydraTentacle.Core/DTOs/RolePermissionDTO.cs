using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    /// <summary>
    /// Rol ile izin arasındaki köprü.
    ///
    /// Köprü tablonun kendi ekranı olmadan Role↔Permission gibi çoka-çok ilişkiler yönetilemez:
    /// izin tanımlayabilir ama kimseye veremezsin. SetConfigurationsForNavigations sayesinde
    /// listede çıplak Guid yerine karşı tarafın okunabilir adı görünür ve formda seçim kutusu
    /// olarak gelir — RequestCategoryResponsiblePositionDTO ile birebir aynı kalıp.
    /// </summary>
    [RegisterAsViewDTO("RolePermission")]
    public class RolePermissionDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public Guid RoleId { get; set; }
        public string Role_Name { get; set; } = string.Empty;
        public Guid Role_Id { get; set; }

        public Guid PermissionId { get; set; }
        public string Permission_Name { get; set; } = string.Empty;
        public Guid Permission_Id { get; set; }

        public RolePermissionDTO()
        {
            SetControllerName("RolePermission");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsForNavigations(
                leftTableKeyName: "RoleId",
                rightTableName: "Role",
                columnNameToDisplay: "Name",
                displayName: "Role"
            );

            SetConfigurationsForNavigations(
                leftTableKeyName: "PermissionId",
                rightTableName: "Permission",
                columnNameToDisplay: "Name",
                displayName: "Permission"
            );

            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
