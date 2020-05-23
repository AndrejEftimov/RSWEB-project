using Microsoft.EntityFrameworkCore.Migrations;

namespace RSWEB_project.Migrations
{
    public partial class Mig1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSemestar",
                table: "Student");

            migrationBuilder.AddColumn<int>(
                name: "CurrentSemester",
                table: "Student",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentSemester",
                table: "Student");

            migrationBuilder.AddColumn<int>(
                name: "CurrentSemestar",
                table: "Student",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
