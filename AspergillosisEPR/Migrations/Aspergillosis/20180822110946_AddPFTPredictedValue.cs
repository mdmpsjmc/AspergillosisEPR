using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPFTPredictedValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PredictedValue",
                table: "PatientPulmonaryFunctionTests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_PatientPulmonaryFunctionTests_PulmonaryFunctionTestId",
                table: "PatientPulmonaryFunctionTests",
                column: "PulmonaryFunctionTestId");

      
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_PatientPulmonaryFunctionTests_PulmonaryFunctionTestId",
                table: "PatientPulmonaryFunctionTests");

            migrationBuilder.DropColumn(
                name: "PredictedValue",
                table: "PatientPulmonaryFunctionTests");
        }
    }
}
