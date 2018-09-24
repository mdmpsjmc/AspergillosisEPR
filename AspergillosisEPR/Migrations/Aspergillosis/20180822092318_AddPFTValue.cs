using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPFTValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTaken",
                table: "PatientPulmonaryFunctionTests",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<decimal>(
                name: "ResultValue",
                table: "PatientPulmonaryFunctionTests",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_PatientPulmonaryFunctionTests_PatientId",
                table: "PatientPulmonaryFunctionTests",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientPulmonaryFunctionTests_Patients_PatientId",
                table: "PatientPulmonaryFunctionTests",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientPulmonaryFunctionTests_Patients_PatientId",
                table: "PatientPulmonaryFunctionTests");

            migrationBuilder.DropIndex(
                name: "IX_PatientPulmonaryFunctionTests_PatientId",
                table: "PatientPulmonaryFunctionTests");

            migrationBuilder.DropColumn(
                name: "ResultValue",
                table: "PatientPulmonaryFunctionTests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTaken",
                table: "PatientPulmonaryFunctionTests",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);
        }
    }
}
