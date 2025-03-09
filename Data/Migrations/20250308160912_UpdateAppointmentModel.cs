using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamProjectOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAppointmentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Coaches_CoachId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_GymHalls_GymHallId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_CoachId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "GroupSession",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CoachId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "GymHallId",
                table: "Appointments",
                newName: "ScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_GymHallId",
                table: "Appointments",
                newName: "IX_Appointments_ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Schedules_ScheduleId",
                table: "Appointments",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Schedules_ScheduleId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "ScheduleId",
                table: "Appointments",
                newName: "GymHallId");

            migrationBuilder.RenameIndex(
                name: "IX_Appointments_ScheduleId",
                table: "Appointments",
                newName: "IX_Appointments_GymHallId");

            migrationBuilder.AddColumn<bool>(
                name: "GroupSession",
                table: "Schedules",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "CoachId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Appointments",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EndTime",
                table: "Appointments",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<TimeOnly>(
                name: "StartTime",
                table: "Appointments",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CoachId",
                table: "Appointments",
                column: "CoachId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Coaches_CoachId",
                table: "Appointments",
                column: "CoachId",
                principalTable: "Coaches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_GymHalls_GymHallId",
                table: "Appointments",
                column: "GymHallId",
                principalTable: "GymHalls",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
