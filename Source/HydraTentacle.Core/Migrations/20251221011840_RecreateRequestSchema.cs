using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HydraTentacle.Core.Migrations
{
    /// <inheritdoc />
    public partial class RecreateRequestSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DROP Existing tables to force clean slate
            migrationBuilder.Sql("IF OBJECT_ID('RequestAssignment', 'U') IS NOT NULL DROP TABLE RequestAssignment");
            migrationBuilder.Sql("IF OBJECT_ID('RequestAttachment', 'U') IS NOT NULL DROP TABLE RequestAttachment");
            migrationBuilder.Sql("IF OBJECT_ID('RequestCategoryResponsiblePosition', 'U') IS NOT NULL DROP TABLE RequestCategoryResponsiblePosition");
            migrationBuilder.Sql("IF OBJECT_ID('Request', 'U') IS NOT NULL DROP TABLE Request");
            migrationBuilder.Sql("IF OBJECT_ID('Employee', 'U') IS NOT NULL DROP TABLE Employee");
            migrationBuilder.Sql("IF OBJECT_ID('RequestCategory', 'U') IS NOT NULL DROP TABLE RequestCategory");
            migrationBuilder.Sql("IF OBJECT_ID('Position', 'U') IS NOT NULL DROP TABLE Position");
            migrationBuilder.Sql("IF OBJECT_ID('OrganizationUnit', 'U') IS NOT NULL DROP TABLE OrganizationUnit");
            
            // Missing Dependency
            migrationBuilder.Sql("IF OBJECT_ID('CustomFile', 'U') IS NOT NULL DROP TABLE CustomFile");

            migrationBuilder.CreateTable(
                name: "CustomFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContainerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LengthAsMb = table.Column<double>(type: "float", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomFile", x => x.Id);
                });

            // Re-Create Tables
            migrationBuilder.CreateTable(
                name: "RequestCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAssignable = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUnit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ManagerPositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationUnit_OrganizationUnit_ParentId",
                        column: x => x.ParentId,
                        principalTable: "OrganizationUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Position",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Position", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Position_OrganizationUnit_OrganizationUnitId",
                        column: x => x.OrganizationUnitId,
                        principalTable: "OrganizationUnit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Position_Position_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActiveEmployee = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employee_SystemUser_SystemUserId",
                        column: x => x.SystemUserId,
                        principalTable: "SystemUser", // Existing
                        principalColumn: "Id");
                     table.ForeignKey(
                        name: "FK_Employee_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Request",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Request", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Request_Employee_CreatedByEmployeeId",
                        column: x => x.CreatedByEmployeeId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Request_RequestCategory_RequestCategoryId",
                        column: x => x.RequestCategoryId,
                        principalTable: "RequestCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RequestAssignment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestAssignment_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestAttachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AddedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestAttachment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestAttachment_CustomFile_FileId",
                        column: x => x.FileId,
                        principalTable: "CustomFile", // Existing
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestAttachment_Request_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Request",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestCategoryResponsiblePosition",
                columns: table => new
                {
                    RequestCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestCategoryResponsiblePosition", x => new { x.RequestCategoryId, x.PositionId });
                    table.ForeignKey(
                        name: "FK_RequestCategoryResponsiblePosition_Position_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Position",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RequestCategoryResponsiblePosition_RequestCategory_RequestCategoryId",
                        column: x => x.RequestCategoryId,
                        principalTable: "RequestCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PositionId",
                table: "Employee",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_SystemUserId",
                table: "Employee",
                column: "SystemUserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnit_ManagerPositionId",
                table: "OrganizationUnit",
                column: "ManagerPositionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUnit_ParentId",
                table: "OrganizationUnit",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Position_OrganizationUnitId",
                table: "Position",
                column: "OrganizationUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Position_ParentId",
                table: "Position",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_CreatedByEmployeeId",
                table: "Request",
                column: "CreatedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestCategoryId",
                table: "Request",
                column: "RequestCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAssignment_AssignedEmployeeId",
                table: "RequestAssignment",
                column: "AssignedEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAssignment_RequestId",
                table: "RequestAssignment",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAttachment_FileId",
                table: "RequestAttachment",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestAttachment_RequestId",
                table: "RequestAttachment",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestCategoryResponsiblePosition_PositionId",
                table: "RequestCategoryResponsiblePosition",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationUnit_Position_ManagerPositionId",
                table: "OrganizationUnit",
                column: "ManagerPositionId",
                principalTable: "Position",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
             // Drop logic
            migrationBuilder.DropTable(name: "RequestAssignment");
            migrationBuilder.DropTable(name: "RequestAttachment");
            migrationBuilder.DropTable(name: "RequestCategoryResponsiblePosition");
            migrationBuilder.DropTable(name: "Request");
            migrationBuilder.DropTable(name: "Employee");
            migrationBuilder.DropTable(name: "Position");
            migrationBuilder.DropTable(name: "RequestCategory");
            migrationBuilder.DropTable(name: "OrganizationUnit");
        }
    }
}
