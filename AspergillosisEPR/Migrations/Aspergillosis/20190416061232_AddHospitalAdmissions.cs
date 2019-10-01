using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddHospitalAdmissions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientHospitalAdmissions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<int>(nullable: false),
                    AdmittedDate = table.Column<DateTime>(nullable: true),
                    DischargedDate = table.Column<DateTime>(nullable: true),
                    AdmissionReason = table.Column<string>(nullable: true),
                    DiagnosisTypeId = table.Column<int>(nullable: true),
                    Length = table.Column<int>(nullable: true),
                    Hospital = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientHospitalAdmissions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PatientHospitalAdmissions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientHospitalAdmissions_PatientId",
                table: "PatientHospitalAdmissions",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientHospitalAdmissions");
        }
    }
}
