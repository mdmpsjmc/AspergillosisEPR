using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPatientSmokingDrinkingStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientSmokingDrinkingStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AlcolholUnits = table.Column<int>(nullable: true),
                    CigarettesPerDay = table.Column<int>(nullable: true),
                    PacksPerYear = table.Column<int>(nullable: true),
                    PatientId = table.Column<int>(nullable: false),
                    SmokingStatusId = table.Column<int>(nullable: false),
                    StartAge = table.Column<int>(nullable: true),
                    StopAge = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSmokingDrinkingStatuses", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PatientSmokingDrinkingStatuses_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientSurgeries_PatientId",
                table: "PatientSurgeries",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSurgeries_SurgeryId",
                table: "PatientSurgeries",
                column: "SurgeryId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientSmokingDrinkingStatuses_PatientId",
                table: "PatientSmokingDrinkingStatuses",
                column: "PatientId",
                unique: true);

     
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropTable(
                name: "PatientSmokingDrinkingStatuses");

            migrationBuilder.DropIndex(
                name: "IX_PatientSurgeries_PatientId",
                table: "PatientSurgeries");

            migrationBuilder.DropIndex(
                name: "IX_PatientSurgeries_SurgeryId",
                table: "PatientSurgeries");
        }
    }
}
