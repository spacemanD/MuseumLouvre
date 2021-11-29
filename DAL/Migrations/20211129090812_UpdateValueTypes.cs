using Microsoft.EntityFrameworkCore.Migrations;

namespace DAL.Migrations
{
    public partial class UpdateValueTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Exhibits");

            migrationBuilder.DropColumn(
                name: "DirectionId",
                table: "Exhibits");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Exhibits");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Authors");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Exhibits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DirectionId",
                table: "Exhibits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Exhibits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CountryId",
                table: "Authors",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
