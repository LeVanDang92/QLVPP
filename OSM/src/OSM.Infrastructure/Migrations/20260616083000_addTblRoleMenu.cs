using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addTblRoleMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenuPermission_AspNetRoles_RoleId",
                table: "RoleMenuPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenuPermission_Menus_MenuId",
                table: "RoleMenuPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenuPermission_Permissions_PermissionId",
                table: "RoleMenuPermission");

            migrationBuilder.CreateTable(
                name: "RoleMenu",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MenuId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenu", x => new { x.RoleId, x.MenuId });
                    table.ForeignKey(
                        name: "FK_RoleMenu_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleMenu_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleMenu_MenuId",
                table: "RoleMenu",
                column: "MenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenuPermission_AspNetRoles_RoleId",
                table: "RoleMenuPermission",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenuPermission_Menus_MenuId",
                table: "RoleMenuPermission",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenuPermission_Permissions_PermissionId",
                table: "RoleMenuPermission",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "PermissionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenuPermission_AspNetRoles_RoleId",
                table: "RoleMenuPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenuPermission_Menus_MenuId",
                table: "RoleMenuPermission");

            migrationBuilder.DropForeignKey(
                name: "FK_RoleMenuPermission_Permissions_PermissionId",
                table: "RoleMenuPermission");

            migrationBuilder.DropTable(
                name: "RoleMenu");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenuPermission_AspNetRoles_RoleId",
                table: "RoleMenuPermission",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenuPermission_Menus_MenuId",
                table: "RoleMenuPermission",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RoleMenuPermission_Permissions_PermissionId",
                table: "RoleMenuPermission",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "PermissionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
