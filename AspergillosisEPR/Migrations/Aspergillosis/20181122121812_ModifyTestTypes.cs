using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ModifyTestTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "TestTypes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UnitOfMeasurementId",
                table: "TestTypes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "TestTypes");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasurementId",
                table: "TestTypes");
        }
    }
}
