using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPatientNACDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientNACDates",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PatientId = table.Column<int>(nullable: false),
                    ProbableStartOfDisease = table.Column<DateTime>(nullable: true),
                    DefiniteStartOfDisease = table.Column<DateTime>(nullable: true),
                    DateOfDiagnosis = table.Column<DateTime>(nullable: true),
                    FirstSeenAtNAC = table.Column<DateTime>(nullable: false),
                    CPABand = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientNACDates", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PatientNACDates_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientNACDates_PatientId",
                table: "PatientNACDates",
                column: "PatientId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientNACDates");
        }
    }
}
