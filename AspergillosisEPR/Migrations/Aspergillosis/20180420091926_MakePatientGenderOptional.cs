using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class MakePatientGenderOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Patients",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateIndex(
                name: "IX_PatientDrugLevel_UnitOfMeasurementId",
                table: "PatientDrugLevel",
                column: "UnitOfMeasurementId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientDrugLevel_UnitOfMeasurements_UnitOfMeasurementId",
                table: "PatientDrugLevel",
                column: "UnitOfMeasurementId",
                principalTable: "UnitOfMeasurements",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientDrugLevel_UnitOfMeasurements_UnitOfMeasurementId",
                table: "PatientDrugLevel");

            migrationBuilder.DropIndex(
                name: "IX_PatientDrugLevel_UnitOfMeasurementId",
                table: "PatientDrugLevel");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "Patients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
