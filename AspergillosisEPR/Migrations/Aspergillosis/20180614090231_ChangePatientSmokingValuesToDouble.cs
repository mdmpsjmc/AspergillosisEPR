using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangePatientSmokingValuesToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "PacksPerYear",
                table: "PatientSmokingDrinkingStatuses",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "CigarettesPerDay",
                table: "PatientSmokingDrinkingStatuses",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PacksPerYear",
                table: "PatientSmokingDrinkingStatuses",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CigarettesPerDay",
                table: "PatientSmokingDrinkingStatuses",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
