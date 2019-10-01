using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangeAdmissionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdmissionReason",
                table: "PatientHospitalAdmissions");

            migrationBuilder.DropColumn(
                name: "AdmittedDate",
                table: "PatientHospitalAdmissions");

            migrationBuilder.DropColumn(
                name: "DiagnosisTypeId",
                table: "PatientHospitalAdmissions");

            migrationBuilder.DropColumn(
                name: "DischargedDate",
                table: "PatientHospitalAdmissions");

            migrationBuilder.DropColumn(
                name: "Hospital",
                table: "PatientHospitalAdmissions");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "PatientHospitalAdmissions");

            migrationBuilder.AddColumn<bool>(
                name: "ICU",
                table: "PatientHospitalAdmissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MoreThanOneAdmission",
                table: "PatientHospitalAdmissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OneOrMoreAdmissions",
                table: "PatientHospitalAdmissions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PreVisit",
                table: "PatientHospitalAdmissions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ICU",
                table: "PatientHospitalAdmissions");

            migrationBuilder.DropColumn(
                name: "MoreThanOneAdmission",
                table: "PatientHospitalAdmissions");

            migrationBuilder.DropColumn(
                name: "OneOrMoreAdmissions",
                table: "PatientHospitalAdmissions");

            migrationBuilder.DropColumn(
                name: "PreVisit",
                table: "PatientHospitalAdmissions");

            migrationBuilder.AddColumn<string>(
                name: "AdmissionReason",
                table: "PatientHospitalAdmissions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AdmittedDate",
                table: "PatientHospitalAdmissions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiagnosisTypeId",
                table: "PatientHospitalAdmissions",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DischargedDate",
                table: "PatientHospitalAdmissions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Hospital",
                table: "PatientHospitalAdmissions",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Length",
                table: "PatientHospitalAdmissions",
                nullable: true);
        }
    }
}
