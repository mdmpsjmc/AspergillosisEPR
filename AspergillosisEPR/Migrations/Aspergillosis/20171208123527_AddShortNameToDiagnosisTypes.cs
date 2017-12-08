using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddShortNameToDiagnosisTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "DiagnosisTypes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientStatusId",
                table: "Patients",
                column: "PatientStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_PatientStatuses_PatientStatusId",
                table: "Patients",
                column: "PatientStatusId",
                principalTable: "PatientStatuses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_PatientStatuses_PatientStatusId",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_PatientStatusId",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "DiagnosisTypes");
        }
    }
}
