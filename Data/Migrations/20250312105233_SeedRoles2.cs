using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExamProjectOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0e476474-e42e-4c34-a69d-75ddb0e3b958");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "507516d9-a183-40e4-a8a8-d34fb4969145");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "692c0a54-cc55-4aae-95f1-39a627cda87a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6bba4dbb-41bd-470b-82bf-2a0a3182c2c8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "73b66cb1-7a99-48c2-85f0-3340b4241813");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e20476e3-3fc2-410a-aa36-05ff0486b153");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "Customer", "CUSTOMER" },
                    { "3", null, "Coach", "COACH" },
                    { "4", null, "Senior coach", "SENIOR COACH" },
                    { "5", null, "Supervisor", "SUPERVISOR" },
                    { "6", null, "Senior supervisor", "SENIOR SUPERVISOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "0e476474-e42e-4c34-a69d-75ddb0e3b958", null, "Senior supervisor", "SENIOR SUPERVISOR" },
                    { "507516d9-a183-40e4-a8a8-d34fb4969145", null, "Coach", "COACH" },
                    { "692c0a54-cc55-4aae-95f1-39a627cda87a", null, "Admin", "ADMIN" },
                    { "6bba4dbb-41bd-470b-82bf-2a0a3182c2c8", null, "Supervisor", "SUPERVISOR" },
                    { "73b66cb1-7a99-48c2-85f0-3340b4241813", null, "Customer", "CUSTOMER" },
                    { "e20476e3-3fc2-410a-aa36-05ff0486b153", null, "Senior coach", "SENIOR COACH" }
                });
        }
    }
}
