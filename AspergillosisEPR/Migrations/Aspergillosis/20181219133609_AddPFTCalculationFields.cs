using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPFTCalculationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Age",
                table: "PatientPulmonaryFunctionTests",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "PatientPulmonaryFunctionTests",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "NormalValue",
                table: "PatientPulmonaryFunctionTests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "PatientPulmonaryFunctionTests");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "PatientPulmonaryFunctionTests");

            migrationBuilder.DropColumn(
                name: "NormalValue",
                table: "PatientPulmonaryFunctionTests");
        }
    }
}
