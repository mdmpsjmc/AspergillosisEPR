using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddCaseReportFormResultsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaseReportFormPatientResults",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormFieldId = table.Column<int>(nullable: false),
                    CaseReportFormId = table.Column<int>(nullable: false),
                    DateAnswer = table.Column<DateTime>(nullable: true),
                    NumericAnswer = table.Column<decimal>(nullable: true),
                    PatientId = table.Column<int>(nullable: false),
                    TextAnswer = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormPatientResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormPatientResults_CaseReportFormFields_CaseReportFormFieldId",
                        column: x => x.CaseReportFormFieldId,
                        principalTable: "CaseReportFormFields",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseReportFormPatientResults_CaseReportForms_CaseReportFormId",
                        column: x => x.CaseReportFormId,
                        principalTable: "CaseReportForms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseReportFormPatientResults_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseReportFormPatientResultOptionChoices",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormOptionChoiceId = table.Column<int>(nullable: false),
                    CaseReportFormPatientResultId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormPatientResultOptionChoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormPatientResultOptionChoices_CaseReportFormPatientResults_CaseReportFormPatientResultId",
                        column: x => x.CaseReportFormPatientResultId,
                        principalTable: "CaseReportFormPatientResults",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormFormSections_CaseReportFormSectionId",
                table: "CaseReportFormFormSections",
                column: "CaseReportFormSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormPatientResultOptionChoices_CaseReportFormPatientResultId",
                table: "CaseReportFormPatientResultOptionChoices",
                column: "CaseReportFormPatientResultId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormPatientResults_CaseReportFormFieldId",
                table: "CaseReportFormPatientResults",
                column: "CaseReportFormFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormPatientResults_CaseReportFormId",
                table: "CaseReportFormPatientResults",
                column: "CaseReportFormId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormPatientResults_PatientId",
                table: "CaseReportFormPatientResults",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormFormSections_CaseReportFormSections_CaseReportFormSectionId",
                table: "CaseReportFormFormSections",
                column: "CaseReportFormSectionId",
                principalTable: "CaseReportFormSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormFormSections_CaseReportFormSections_CaseReportFormSectionId",
                table: "CaseReportFormFormSections");

            migrationBuilder.DropTable(
                name: "CaseReportFormPatientResultOptionChoices");

            migrationBuilder.DropTable(
                name: "CaseReportFormPatientResults");

            migrationBuilder.DropIndex(
                name: "IX_CaseReportFormFormSections_CaseReportFormSectionId",
                table: "CaseReportFormFormSections");
        }
    }
}
