using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addSortOrderToMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IconIndex",
                table: "Menus",
                newName: "IconClass");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Menus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Menus",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Menus");

            migrationBuilder.RenameColumn(
                name: "IconClass",
                table: "Menus",
                newName: "IconIndex");
        }
    }
}
