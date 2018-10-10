using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddTestTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PatientNACDates_PatientId",
                table: "PatientNACDates");

            migrationBuilder.CreateTable(
                name: "TestTypes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PatientTestResult",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TestTypeId = table.Column<int>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    UnitOfMeasurementId = table.Column<int>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Range = table.Column<string>(nullable: true),
                    SampleId = table.Column<string>(nullable: true),
                    DateTaken = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientTestResult", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PatientTestResult_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientTestResult_TestTypes_TestTypeId",
                        column: x => x.TestTypeId,
                        principalTable: "TestTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientTestResult_UnitOfMeasurements_UnitOfMeasurementId",
                        column: x => x.UnitOfMeasurementId,
                        principalTable: "UnitOfMeasurements",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientNACDates_PatientId",
                table: "PatientNACDates",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTestResult_PatientId",
                table: "PatientTestResult",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTestResult_TestTypeId",
                table: "PatientTestResult",
                column: "TestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientTestResult_UnitOfMeasurementId",
                table: "PatientTestResult",
                column: "UnitOfMeasurementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientTestResult");

            migrationBuilder.DropTable(
                name: "TestTypes");

            migrationBuilder.DropIndex(
                name: "IX_PatientNACDates_PatientId",
                table: "PatientNACDates");

            migrationBuilder.CreateIndex(
                name: "IX_PatientNACDates_PatientId",
                table: "PatientNACDates",
                column: "PatientId",
                unique: true);
        }
    }
}
