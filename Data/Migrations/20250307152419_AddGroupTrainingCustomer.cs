using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExamProjectOne.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupTrainingCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupTrainings_Customers_CustomerId",
                table: "GroupTrainings");

            migrationBuilder.DropIndex(
                name: "IX_GroupTrainings_CustomerId",
                table: "GroupTrainings");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "GroupTrainings");

            migrationBuilder.CreateTable(
                name: "GroupTrainingCustomers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupTrainingId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupTrainingCustomers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupTrainingCustomers_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_GroupTrainingCustomers_GroupTrainings_GroupTrainingId",
                        column: x => x.GroupTrainingId,
                        principalTable: "GroupTrainings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupTrainingCustomers_CustomerId",
                table: "GroupTrainingCustomers",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupTrainingCustomers_GroupTrainingId",
                table: "GroupTrainingCustomers",
                column: "GroupTrainingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupTrainingCustomers");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "GroupTrainings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GroupTrainings_CustomerId",
                table: "GroupTrainings",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_GroupTrainings_Customers_CustomerId",
                table: "GroupTrainings",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
