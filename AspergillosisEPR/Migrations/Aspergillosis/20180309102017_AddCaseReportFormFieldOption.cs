using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddCaseReportFormFieldOption : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CaseReportFormFieldTypes",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CaseReportFormCategories",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            

            migrationBuilder.CreateIndex(
                name: "IX_PatientMeasurements_PatientId",
                table: "PatientMeasurements",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormFieldOptions_CaseReportFormFieldId",
                table: "CaseReportFormFieldOptions",
                column: "CaseReportFormFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormFieldOptions_CaseReportFormOptionChoiceId",
                table: "CaseReportFormFieldOptions",
                column: "CaseReportFormOptionChoiceId");

      
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientMeasurements_Patients_PatientId",
                table: "PatientMeasurements");

            migrationBuilder.DropTable(
                name: "CaseReportFormFieldOptions");

            migrationBuilder.DropIndex(
                name: "IX_PatientMeasurements_PatientId",
                table: "PatientMeasurements");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CaseReportFormFieldTypes",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CaseReportFormCategories",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
