using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangePatientRadiologyNotes2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SourceSystemGUID",
                table: "PatientRadiologyNotes",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "SourceSystemGUID",
                table: "PatientRadiologyNotes",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
