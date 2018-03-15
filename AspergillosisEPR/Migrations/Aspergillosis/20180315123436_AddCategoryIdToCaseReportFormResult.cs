using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddCategoryIdToCaseReportFormResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormPatientResults_CaseReportFormResults_CaseReportFormResultID",
                table: "CaseReportFormPatientResults");

            migrationBuilder.RenameColumn(
                name: "CaseReportFormResultID",
                table: "CaseReportFormPatientResults",
                newName: "CaseReportFormResultId");

            migrationBuilder.RenameIndex(
                name: "IX_CaseReportFormPatientResults_CaseReportFormResultID",
                table: "CaseReportFormPatientResults",
                newName: "IX_CaseReportFormPatientResults_CaseReportFormResultId");

            migrationBuilder.AddColumn<int>(
                name: "CaseReportFormCategoryId",
                table: "CaseReportFormResults",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CaseReportFormResultId",
                table: "CaseReportFormPatientResults",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormPatientResults_CaseReportFormResults_CaseReportFormResultId",
                table: "CaseReportFormPatientResults");

            migrationBuilder.DropColumn(
                name: "CaseReportFormCategoryId",
                table: "CaseReportFormResults");

            migrationBuilder.RenameColumn(
                name: "CaseReportFormResultId",
                table: "CaseReportFormPatientResults",
                newName: "CaseReportFormResultID");

            migrationBuilder.RenameIndex(
                name: "IX_CaseReportFormPatientResults_CaseReportFormResultId",
                table: "CaseReportFormPatientResults",
                newName: "IX_CaseReportFormPatientResults_CaseReportFormResultID");

            migrationBuilder.AlterColumn<int>(
                name: "CaseReportFormResultID",
                table: "CaseReportFormPatientResults",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormPatientResults_CaseReportFormResults_CaseReportFormResultID",
                table: "CaseReportFormPatientResults",
                column: "CaseReportFormResultID",
                principalTable: "CaseReportFormResults",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
