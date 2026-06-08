using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OSM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addCodeTableData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Code_Data",
                columns: table => new
                {
                    Data_Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Table_Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Data_Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Data_Value = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Sort_Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Code_Data", x => x.Data_Id);
                });

            migrationBuilder.CreateTable(
                name: "Code_Table",
                columns: table => new
                {
                    Table_Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Table_Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Table_Group = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Is_System = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Code_Table", x => x.Table_Code);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Code_Data");

            migrationBuilder.DropTable(
                name: "Code_Table");
        }
    }
}
