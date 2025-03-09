using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamProjectOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class addTitleToSchedual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Schedules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Appointments");
        }
    }
}
