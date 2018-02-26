using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddCaseReportForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaseReportFormCategories",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormCategories", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CaseReportFormFieldTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormFieldTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CaseReportFormOptionGroups",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormOptionGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "CaseReportFormResults",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormCategoryID = table.Column<int>(nullable: true),
                    CaseReportFormResultCategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormResults_CaseReportFormCategories_CaseReportFormCategoryID",
                        column: x => x.CaseReportFormCategoryID,
                        principalTable: "CaseReportFormCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseReportFormOptionChoices",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormOptionGroupId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormOptionChoices", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormOptionChoices_CaseReportFormOptionGroups_CaseReportFormOptionGroupId",
                        column: x => x.CaseReportFormOptionGroupId,
                        principalTable: "CaseReportFormOptionGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CaseReportFormSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormResultID = table.Column<int>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormSections_CaseReportFormResults_CaseReportFormResultID",
                        column: x => x.CaseReportFormResultID,
                        principalTable: "CaseReportFormResults",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaseReportFormFields",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormFieldTypeId = table.Column<int>(nullable: false),
                    CaseReportFormSectionID = table.Column<int>(nullable: true),
                    Label = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormFields", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormFields_CaseReportFormFieldTypes_CaseReportFormFieldTypeId",
                        column: x => x.CaseReportFormFieldTypeId,
                        principalTable: "CaseReportFormFieldTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionID",
                        column: x => x.CaseReportFormSectionID,
                        principalTable: "CaseReportFormSections",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });



            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormFields_CaseReportFormFieldTypeId",
                table: "CaseReportFormFields",
                column: "CaseReportFormFieldTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormFields_CaseReportFormSectionID",
                table: "CaseReportFormFields",
                column: "CaseReportFormSectionID");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormOptionChoices_CaseReportFormOptionGroupId",
                table: "CaseReportFormOptionChoices",
                column: "CaseReportFormOptionGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormResults_CaseReportFormCategoryID",
                table: "CaseReportFormResults",
                column: "CaseReportFormCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormSections_CaseReportFormResultID",
                table: "CaseReportFormSections",
                column: "CaseReportFormResultID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientExaminations_PatientImmunoglobulins_PatientImmunoglobulinId",
                table: "PatientExaminations");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientExaminations_PatientMeasurements_PatientMeasurementId",
                table: "PatientExaminations");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientExaminations_PatientRadiologyFindings_PatientRadiologyFinidingId",
                table: "PatientExaminations");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientExaminations_PatientSTGQuestionnaires_PatientSTGQuestionnaireId",
                table: "PatientExaminations");

            migrationBuilder.DropTable(
                name: "CaseReportFormFields");

            migrationBuilder.DropTable(
                name: "CaseReportFormOptionChoices");

            migrationBuilder.DropTable(
                name: "CaseReportFormFieldTypes");

            migrationBuilder.DropTable(
                name: "CaseReportFormSections");

            migrationBuilder.DropTable(
                name: "CaseReportFormOptionGroups");

            migrationBuilder.DropTable(
                name: "CaseReportFormResults");

            migrationBuilder.DropTable(
                name: "CaseReportFormCategories");

            migrationBuilder.DropIndex(
                name: "IX_PatientExaminations_PatientImmunoglobulinId",
                table: "PatientExaminations");

            migrationBuilder.DropIndex(
                name: "IX_PatientExaminations_PatientMeasurementId",
                table: "PatientExaminations");

            migrationBuilder.DropIndex(
                name: "IX_PatientExaminations_PatientRadiologyFinidingId",
                table: "PatientExaminations");

            migrationBuilder.DropIndex(
                name: "IX_PatientExaminations_PatientSTGQuestionnaireId",
                table: "PatientExaminations");
        }
    }
}
