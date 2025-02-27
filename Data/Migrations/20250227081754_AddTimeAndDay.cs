using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamProjectOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeAndDay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShiftTime",
                table: "Supervisors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkDay",
                table: "Supervisors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShiftTime",
                table: "Coaches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkDay",
                table: "Coaches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftTime",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "WorkDay",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "ShiftTime",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "WorkDay",
                table: "Coaches");
        }
    }
}
