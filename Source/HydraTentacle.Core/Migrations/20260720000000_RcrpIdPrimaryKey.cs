using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HydraTentacle.Core.Migrations
{
    /// <inheritdoc />
    public partial class RcrpIdPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // RequestCategoryResponsiblePosition: composite PK (RequestCategoryId, PositionId)
            // -> BaseObject konvansiyonu geregi PK = Id, ikili ise unique index olarak korunur.

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestCategoryResponsiblePosition",
                table: "RequestCategoryResponsiblePosition");

            // FixRequest2 "Id" kolonunu defaultValue = Guid.Empty ile ekledi.
            // Mevcut satirlarin hepsi ayni Id'ye sahip -> PK eklemeden once benzersizlestir.
            migrationBuilder.Sql(
                "UPDATE [RequestCategoryResponsiblePosition] SET [Id] = NEWID() " +
                "WHERE [Id] = '00000000-0000-0000-0000-000000000000';");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestCategoryResponsiblePosition",
                table: "RequestCategoryResponsiblePosition",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCategoryResponsiblePosition_RequestCategoryId_PositionId",
                table: "RequestCategoryResponsiblePosition",
                columns: new[] { "RequestCategoryId", "PositionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestCategoryResponsiblePosition_RequestCategoryId_PositionId",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RequestCategoryResponsiblePosition",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RequestCategoryResponsiblePosition",
                table: "RequestCategoryResponsiblePosition",
                columns: new[] { "RequestCategoryId", "PositionId" });
        }
    }
}
