using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddCaseReportFormResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaseReportFormResultID",
                table: "CaseReportFormPatientResults",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CaseReportFormResults",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormId = table.Column<int>(nullable: false),
                    DateTaken = table.Column<DateTime>(nullable: false),
                    PatientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormResults_CaseReportForms_CaseReportFormId",
                        column: x => x.CaseReportFormId,
                        principalTable: "CaseReportForms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseReportFormResults_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormPatientResults_CaseReportFormResultID",
                table: "CaseReportFormPatientResults",
                column: "CaseReportFormResultID");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormResults_CaseReportFormId",
                table: "CaseReportFormResults",
                column: "CaseReportFormId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormResults_PatientId",
                table: "CaseReportFormResults",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormPatientResults_CaseReportFormResults_CaseReportFormResultID",
                table: "CaseReportFormPatientResults",
                column: "CaseReportFormResultID",
                principalTable: "CaseReportFormResults",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormPatientResults_CaseReportFormResults_CaseReportFormResultID",
                table: "CaseReportFormPatientResults");

            migrationBuilder.DropTable(
                name: "CaseReportFormResults");

            migrationBuilder.DropIndex(
                name: "IX_CaseReportFormPatientResults_CaseReportFormResultID",
                table: "CaseReportFormPatientResults");

            migrationBuilder.DropColumn(
                name: "CaseReportFormResultID",
                table: "CaseReportFormPatientResults");
        }
    }
}
