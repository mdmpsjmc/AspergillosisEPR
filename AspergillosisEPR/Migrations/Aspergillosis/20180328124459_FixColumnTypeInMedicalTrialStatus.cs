using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class FixColumnTypeInMedicalTrialStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MedicalTrialStatuses",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "MedicalTrialStatuses",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
