using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class OrderGUIDToTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceSystemGUID",
                table: "PatientImmunoglobulins",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientICD10Diagnoses_PatientId",
                table: "PatientICD10Diagnoses",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientICD10Diagnoses_Patients_PatientId",
                table: "PatientICD10Diagnoses",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientICD10Diagnoses_Patients_PatientId",
                table: "PatientICD10Diagnoses");

            migrationBuilder.DropIndex(
                name: "IX_PatientICD10Diagnoses_PatientId",
                table: "PatientICD10Diagnoses");

            migrationBuilder.DropColumn(
                name: "SourceSystemGUID",
                table: "PatientImmunoglobulins");
        }
    }
}
