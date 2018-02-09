using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class CreateExaminationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientExaminations",
                columns: table => new
                {                    
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Discriminator = table.Column<string>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    PatientRadiologyFinidingId = table.Column<int>(nullable: true),
                    PatientSTGQuestionnaireId = table.Column<int>(nullable: true),
                    PatientImmunoglobulinId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientExaminations", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PatientVisits",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<int>(nullable: false),
                    VisitDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientVisits", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyFindings_ChestDistributionId",
                table: "PatientRadiologyFindings",
                column: "ChestDistributionId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyFindings_ChestLocationId",
                table: "PatientRadiologyFindings",
                column: "ChestLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyFindings_FindingId",
                table: "PatientRadiologyFindings",
                column: "FindingId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyFindings_GradeId",
                table: "PatientRadiologyFindings",
                column: "GradeId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyFindings_PatientId",
                table: "PatientRadiologyFindings",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyFindings_RadiologyTypeId",
                table: "PatientRadiologyFindings",
                column: "RadiologyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyFindings_TreatmentResponseId",
                table: "PatientRadiologyFindings",
                column: "TreatmentResponseId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyFindings_ChestDistributions_ChestDistributionId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyFindings_ChestLocations_ChestLocationId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyFindings_Findings_FindingId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyFindings_Grades_GradeId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyFindings_Patients_PatientId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyFindings_RadiologyTypes_RadiologyTypeId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyFindings_TreatmentResponses_TreatmentResponseId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropTable(
                name: "PatientExaminations");

            migrationBuilder.DropTable(
                name: "PatientVisits");


          
        }
    }
}
