using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HydraTentacle.Core.Migrations
{
    /// <inheritdoc />
    public partial class FixRequest2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestAssignment");

            migrationBuilder.RenameColumn(
                name: "EntityName",
                table: "Log",
                newName: "EntityType");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedDate",
                table: "RequestCategoryResponsiblePosition",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RequestCategoryResponsiblePosition",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "RequestCategoryResponsiblePosition",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RequestCategoryResponsiblePosition",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "RequestCategoryResponsiblePosition",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RequestCategoryResponsiblePosition",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "RequestCategoryResponsiblePosition",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerEmployeeId",
                table: "Request",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                table: "Log",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlatformId",
                table: "Log",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EntityId",
                table: "CustomFile",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Platform",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectType = table.Column<int>(type: "int", nullable: false),
                    FrameworkVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platform", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Request_OwnerEmployeeId",
                table: "Request",
                column: "OwnerEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Log_PlatformId",
                table: "Log",
                column: "PlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_Log_Platform_PlatformId",
                table: "Log",
                column: "PlatformId",
                principalTable: "Platform",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_Employee_OwnerEmployeeId",
                table: "Request",
                column: "OwnerEmployeeId",
                principalTable: "Employee",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Log_Platform_PlatformId",
                table: "Log");

            migrationBuilder.DropForeignKey(
                name: "FK_Request_Employee_OwnerEmployeeId",
                table: "Request");

            migrationBuilder.DropTable(
                name: "Platform");

            migrationBuilder.DropIndex(
                name: "IX_Request_OwnerEmployeeId",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Log_PlatformId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "AddedDate",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "RequestCategoryResponsiblePosition");

            migrationBuilder.DropColumn(
                name: "OwnerEmployeeId",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "Log");

            migrationBuilder.DropColumn(
                name: "PlatformId",
                table: "Log");

            migrationBuilder.RenameColumn(
                name: "EntityType",
                table: "Log",
                newName: "EntityName");

            migrationBuilder.AlterColumn<Guid>(
                name: "EntityId",
                table: "CustomFile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RequestAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAssignment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAssignment_Employee_AssignedEmployeeId",
                        column: x => x.AssignedEmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestAssignment_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestAssignment_AssignedEmployeeId",
                table: "RequestAssignment",
                column: "AssignedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAssignment_RequestId",
                table: "RequestAssignment",
                column: "RequestId");
        }
    }
}
