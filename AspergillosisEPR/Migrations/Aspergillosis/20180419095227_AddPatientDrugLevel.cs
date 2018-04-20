using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPatientDrugLevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTrials_MedicalTrialStatuses_MedicalTrialStatusId",
                table: "MedicalTrials");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTrials_MedicalTrialTypes_MedicalTrialTypeId",
                table: "MedicalTrials");

            migrationBuilder.AddColumn<string>(
                name: "NhsNumber",
                table: "Patients",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IdentifiedDate",
                table: "PatientMedicalTrials",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "MedicalTrials",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MedicalTrialTypeId",
                table: "MedicalTrials",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MedicalTrialStatusId",
                table: "MedicalTrials",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "MedicalTrialPrincipalInvestigators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "MedicalTrialPrincipalInvestigators",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalTrials_PatientMedicalTrialStatusId",
                table: "PatientMedicalTrials",
                column: "PatientMedicalTrialStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalTrials_MedicalTrialStatuses_MedicalTrialStatusId",
                table: "MedicalTrials",
                column: "MedicalTrialStatusId",
                principalTable: "MedicalTrialStatuses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalTrials_MedicalTrialTypes_MedicalTrialTypeId",
                table: "MedicalTrials",
                column: "MedicalTrialTypeId",
                principalTable: "MedicalTrialTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientMedicalTrials_MedicalTrialPatientStatuses_PatientMedicalTrialStatusId",
                table: "PatientMedicalTrials",
                column: "PatientMedicalTrialStatusId",
                principalTable: "MedicalTrialPatientStatuses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTrials_MedicalTrialStatuses_MedicalTrialStatusId",
                table: "MedicalTrials");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTrials_MedicalTrialTypes_MedicalTrialTypeId",
                table: "MedicalTrials");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientMedicalTrials_MedicalTrialPatientStatuses_PatientMedicalTrialStatusId",
                table: "PatientMedicalTrials");

            migrationBuilder.DropIndex(
                name: "IX_PatientMedicalTrials_PatientMedicalTrialStatusId",
                table: "PatientMedicalTrials");

            migrationBuilder.DropColumn(
                name: "NhsNumber",
                table: "Patients");

            migrationBuilder.AlterColumn<DateTime>(
                name: "IdentifiedDate",
                table: "PatientMedicalTrials",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                table: "MedicalTrials",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "MedicalTrialTypeId",
                table: "MedicalTrials",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "MedicalTrialStatusId",
                table: "MedicalTrials",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "MedicalTrialPrincipalInvestigators",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "MedicalTrialPrincipalInvestigators",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalTrials_MedicalTrialStatuses_MedicalTrialStatusId",
                table: "MedicalTrials",
                column: "MedicalTrialStatusId",
                principalTable: "MedicalTrialStatuses",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalTrials_MedicalTrialTypes_MedicalTrialTypeId",
                table: "MedicalTrials",
                column: "MedicalTrialTypeId",
                principalTable: "MedicalTrialTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
