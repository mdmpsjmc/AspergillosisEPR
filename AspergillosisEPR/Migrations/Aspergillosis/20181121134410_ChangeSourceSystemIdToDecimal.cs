using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangeSourceSystemIdToDecimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "SourceSystemGUID",
                table: "PatientImmunoglobulins",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SourceSystemGUID",
                table: "PatientImmunoglobulins",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }
    }
}
