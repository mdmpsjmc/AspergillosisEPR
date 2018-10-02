using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ModifyPatientImmunnoglobulines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Range",
                table: "PatientImmunoglobulins",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SampleId",
                table: "PatientImmunoglobulins",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Range",
                table: "PatientImmunoglobulins");

            migrationBuilder.DropColumn(
                name: "SampleId",
                table: "PatientImmunoglobulins");
        }
    }
}
