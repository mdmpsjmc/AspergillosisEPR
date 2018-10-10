using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangeDistanceFromHospitalToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "DistanceFromWythenshawe",
                table: "Patients",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DistanceFromWythenshawe",
                table: "Patients",
                nullable: true,
                oldClrType: typeof(double));
        }
    }
}
