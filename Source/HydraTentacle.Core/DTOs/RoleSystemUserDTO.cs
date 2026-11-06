using Hydra.DTOs.ViewConfigurations;
using Hydra.DTOs.ViewDTOs;
using Hydra.Utils;

namespace HydraTentacle.Core.DTOs
{
    /// <summary>
    /// Rol ile kullanıcı arasındaki köprü.
    ///
    /// Köprü tablonun kendi ekranı olmadan Role↔Permission gibi çoka-çok ilişkiler yönetilemez:
    /// izin tanımlayabilir ama kimseye veremezsin. SetConfigurationsForNavigations sayesinde
    /// listede çıplak Guid yerine karşı tarafın okunabilir adı görünür ve formda seçim kutusu
    /// olarak gelir — RequestCategoryResponsiblePositionDTO ile birebir aynı kalıp.
    /// </summary>
    [RegisterAsViewDTO("RoleSystemUser")]
    public class RoleSystemUserDTO : Hydra.DTOs.ViewDTOs.ViewDTO
    {
        public Guid RoleId { get; set; }
        public string Role_Name { get; set; } = string.Empty;
        public Guid Role_Id { get; set; }

        public Guid UserId { get; set; }
        public string SystemUser_Email { get; set; } = string.Empty;
        public Guid SystemUser_Id { get; set; }

        public RoleSystemUserDTO()
        {
            SetControllerName("RoleSystemUser");
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
                leftTableKeyName: "UserId",
                rightTableName: "SystemUser",
                columnNameToDisplay: "Email",
                displayName: "User"
            );

            SetConfigurationsForBaseObjectMembers();

            return this;
        }
    }
}
