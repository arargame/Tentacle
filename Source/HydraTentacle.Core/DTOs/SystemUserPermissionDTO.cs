using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    /// <summary>
    /// Kullanıcıya doğrudan verilen izin köprüsü (rolden bağımsız).
    ///
    /// Köprü tablonun kendi ekranı olmadan Role↔Permission gibi çoka-çok ilişkiler yönetilemez:
    /// izin tanımlayabilir ama kimseye veremezsin. SetConfigurationsForNavigations sayesinde
    /// listede çıplak Guid yerine karşı tarafın okunabilir adı görünür ve formda seçim kutusu
    /// olarak gelir — RequestCategoryResponsiblePositionDTO ile birebir aynı kalıp.
    /// </summary>
    [RegisterAsViewDTO("SystemUserPermission")]
    public class SystemUserPermissionDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public Guid SystemUserId { get; set; }
        public string SystemUser_Email { get; set; } = string.Empty;
        public Guid SystemUser_Id { get; set; }

        public Guid PermissionId { get; set; }
        public string Permission_Name { get; set; } = string.Empty;
        public Guid Permission_Id { get; set; }

        public SystemUserPermissionDTO()
        {
            SetControllerName("SystemUserPermission");
        }

        public override Hydra.DTOs.DTO LoadConfigurations()
        {
            SetConfigurationsForNavigations(
                leftTableKeyName: "SystemUserId",
                rightTableName: "SystemUser",
                columnNameToDisplay: "Email",
                displayName: "User"
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
