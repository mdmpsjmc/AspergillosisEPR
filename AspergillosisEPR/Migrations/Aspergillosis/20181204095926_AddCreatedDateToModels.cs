using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddCreatedDateToModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PatientTestResult",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PatientRadiologyNotes",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PatientImmunoglobulins",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "PatientDrugLevel",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientRadiologyNotes_RadiologyTypeId",
                table: "PatientRadiologyNotes",
                column: "RadiologyTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientRadiologyNotes_RadiologyTypes_RadiologyTypeId",
                table: "PatientRadiologyNotes",
                column: "RadiologyTypeId",
                principalTable: "RadiologyTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyNotes_RadiologyTypes_RadiologyTypeId",
                table: "PatientRadiologyNotes");

            migrationBuilder.DropIndex(
                name: "IX_PatientRadiologyNotes_RadiologyTypeId",
                table: "PatientRadiologyNotes");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PatientTestResult");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PatientRadiologyNotes");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PatientImmunoglobulins");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "PatientDrugLevel");
        }
    }
}
