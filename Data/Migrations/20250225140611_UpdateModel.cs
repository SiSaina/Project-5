using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamProjectOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WorkingHour",
                table: "Coaches",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Supervisors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "Supervisors",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Supervisors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "Supervisors",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Coaches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "Coaches",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Coaches",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Supervisors");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Coaches");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Coaches",
                newName: "WorkingHour");
        }
    }
}
