using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPatientMeasurements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PatientSTGQuestionnaireId",
                table: "PatientExaminations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PatientRadiologyFinidingId",
                table: "PatientExaminations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PatientImmunoglobulinId",
                table: "PatientExaminations",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PatientMeasurements",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Height = table.Column<double>(type: "decimal(10,2)", nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    Weight = table.Column<double>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMeasurements", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientVisits_PatientId",
                table: "PatientVisits",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientExaminations_PatientVisitId",
                table: "PatientExaminations",
                column: "PatientVisitId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientExaminations_PatientVisits_PatientVisitId",
                table: "PatientExaminations",
                column: "PatientVisitId",
                principalTable: "PatientVisits",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientVisits_Patients_PatientId",
                table: "PatientVisits",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientExaminations_PatientVisits_PatientVisitId",
                table: "PatientExaminations");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientVisits_Patients_PatientId",
                table: "PatientVisits");

            migrationBuilder.DropTable(
                name: "PatientMeasurements");

            migrationBuilder.DropIndex(
                name: "IX_PatientVisits_PatientId",
                table: "PatientVisits");

            migrationBuilder.DropIndex(
                name: "IX_PatientExaminations_PatientVisitId",
                table: "PatientExaminations");

            migrationBuilder.AlterColumn<int>(
                name: "PatientSTGQuestionnaireId",
                table: "PatientExaminations",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "PatientRadiologyFinidingId",
                table: "PatientExaminations",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "PatientImmunoglobulinId",
                table: "PatientExaminations",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
