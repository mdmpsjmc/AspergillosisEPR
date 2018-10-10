using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class MakeRadiologyFieldsOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<int>(
                name: "TreatmentResponseId",
                table: "PatientRadiologyFindings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "GradeId",
                table: "PatientRadiologyFindings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTaken",
                table: "PatientRadiologyFindings",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<int>(
                name: "ChestLocationId",
                table: "PatientRadiologyFindings",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<int>(
                name: "ChestDistributionId",
                table: "PatientRadiologyFindings",
                nullable: true,
                oldClrType: typeof(int));
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
                name: "FK_PatientRadiologyFindings_Grades_GradeId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientRadiologyFindings_TreatmentResponses_TreatmentResponseId",
                table: "PatientRadiologyFindings");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientSmokingDrinkingStatuses_SmokingStatuses_SmokingStatusId",
                table: "PatientSmokingDrinkingStatuses");

            migrationBuilder.DropIndex(
                name: "IX_PatientSmokingDrinkingStatuses_SmokingStatusId",
                table: "PatientSmokingDrinkingStatuses");

            migrationBuilder.AlterColumn<int>(
                name: "TreatmentResponseId",
                table: "PatientRadiologyFindings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GradeId",
                table: "PatientRadiologyFindings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateTaken",
                table: "PatientRadiologyFindings",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChestLocationId",
                table: "PatientRadiologyFindings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ChestDistributionId",
                table: "PatientRadiologyFindings",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

         
        }
    }
}
