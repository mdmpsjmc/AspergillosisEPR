using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddOtherMedicalTrialsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MedicalTrialPrincipalInvestigatorId",
                table: "MedicalTrials",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicalTrialStatusId",
                table: "MedicalTrials",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MedicalTrialTypeId",
                table: "MedicalTrials",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Number",
                table: "MedicalTrials",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RECNumber",
                table: "MedicalTrials",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RandDNumber",
                table: "MedicalTrials",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicalTrialPatientStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalTrialPatientStatuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MedicalTrialStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalTrialStatuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MedicalTrialTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalTrialTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PatientMedicalTrials",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Consented = table.Column<bool>(nullable: false),
                    ConsentedDate = table.Column<DateTime>(nullable: true),
                    IdentifiedDate = table.Column<DateTime>(nullable: true),
                    MedicalTrialId = table.Column<int>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    PatientMedicalTrialStatusId = table.Column<int>(nullable: false),
                    RecruitedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientMedicalTrials", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PatientMedicalTrials_MedicalTrials_MedicalTrialId",
                        column: x => x.MedicalTrialId,
                        principalTable: "MedicalTrials",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientMedicalTrials_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonTitles",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonTitles", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "MedicalTrialPrincipalInvestigators",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    PersonTitleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalTrialPrincipalInvestigators", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MedicalTrialPrincipalInvestigators_PersonTitles_PersonTitleId",
                        column: x => x.PersonTitleId,
                        principalTable: "PersonTitles",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalTrials_MedicalTrialPrincipalInvestigatorId",
                table: "MedicalTrials",
                column: "MedicalTrialPrincipalInvestigatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalTrials_MedicalTrialStatusId",
                table: "MedicalTrials",
                column: "MedicalTrialStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalTrials_MedicalTrialTypeId",
                table: "MedicalTrials",
                column: "MedicalTrialTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalTrialPrincipalInvestigators_PersonTitleId",
                table: "MedicalTrialPrincipalInvestigators",
                column: "PersonTitleId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalTrials_MedicalTrialId",
                table: "PatientMedicalTrials",
                column: "MedicalTrialId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientMedicalTrials_PatientId",
                table: "PatientMedicalTrials",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalTrials_MedicalTrialPrincipalInvestigators_MedicalTrialPrincipalInvestigatorId",
                table: "MedicalTrials",
                column: "MedicalTrialPrincipalInvestigatorId",
                principalTable: "MedicalTrialPrincipalInvestigators",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTrials_MedicalTrialPrincipalInvestigators_MedicalTrialPrincipalInvestigatorId",
                table: "MedicalTrials");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTrials_MedicalTrialStatuses_MedicalTrialStatusId",
                table: "MedicalTrials");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalTrials_MedicalTrialTypes_MedicalTrialTypeId",
                table: "MedicalTrials");

            migrationBuilder.DropTable(
                name: "MedicalTrialPatientStatuses");

            migrationBuilder.DropTable(
                name: "MedicalTrialPrincipalInvestigators");

            migrationBuilder.DropTable(
                name: "MedicalTrialStatuses");

            migrationBuilder.DropTable(
                name: "MedicalTrialTypes");

            migrationBuilder.DropTable(
                name: "PatientMedicalTrials");

            migrationBuilder.DropTable(
                name: "PersonTitles");

            migrationBuilder.DropIndex(
                name: "IX_MedicalTrials_MedicalTrialPrincipalInvestigatorId",
                table: "MedicalTrials");

            migrationBuilder.DropIndex(
                name: "IX_MedicalTrials_MedicalTrialStatusId",
                table: "MedicalTrials");

            migrationBuilder.DropIndex(
                name: "IX_MedicalTrials_MedicalTrialTypeId",
                table: "MedicalTrials");

            migrationBuilder.DropColumn(
                name: "MedicalTrialPrincipalInvestigatorId",
                table: "MedicalTrials");

            migrationBuilder.DropColumn(
                name: "MedicalTrialStatusId",
                table: "MedicalTrials");

            migrationBuilder.DropColumn(
                name: "MedicalTrialTypeId",
                table: "MedicalTrials");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "MedicalTrials");

            migrationBuilder.DropColumn(
                name: "RECNumber",
                table: "MedicalTrials");

            migrationBuilder.DropColumn(
                name: "RandDNumber",
                table: "MedicalTrials");
        }
    }
}
