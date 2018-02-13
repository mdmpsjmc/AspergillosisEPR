using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPatientMeasurementToExaminations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "PatientMeasurements",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "Height",
                table: "PatientMeasurements",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AddColumn<int>(
                name: "PatientMeasurementId",
                table: "PatientExaminations",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PatientMeasurementId",
                table: "PatientExaminations");

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "PatientMeasurements",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "Height",
                table: "PatientMeasurements",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
