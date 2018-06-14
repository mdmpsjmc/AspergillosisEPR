using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangePatientSmokingDrinkingAlcoholUnitsToDouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "AlcolholUnits",
                table: "PatientSmokingDrinkingStatuses",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AlcolholUnits",
                table: "PatientSmokingDrinkingStatuses",
                nullable: true,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
