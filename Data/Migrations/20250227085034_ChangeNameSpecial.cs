using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamProjectOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeNameSpecial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.RenameColumn(
                name: "Specialization",
                table: "Coaches",
                newName: "Specialize");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Specialize",
                table: "Coaches",
                newName: "Specialization");
        }
    }
}
