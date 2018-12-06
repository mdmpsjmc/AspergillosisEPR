using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangePatientRadiologyNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateTaken",
                table: "PatientRadiologyNotes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "SourceSystemGUID",
                table: "PatientRadiologyNotes",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyNotes_PatientId",
                table: "PatientRadiologyNotes",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientRadiologyNotes_Patients_PatientId",
                table: "PatientRadiologyNotes",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyNotes_Patients_PatientId",
                table: "PatientRadiologyNotes");

            migrationBuilder.DropIndex(
                name: "IX_PatientRadiologyNotes_PatientId",
                table: "PatientRadiologyNotes");

            migrationBuilder.DropColumn(
                name: "DateTaken",
                table: "PatientRadiologyNotes");

            migrationBuilder.DropColumn(
                name: "SourceSystemGUID",
                table: "PatientRadiologyNotes");
        }
    }
}
