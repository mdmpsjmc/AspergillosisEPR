using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPatientRadiologyFindingsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientRadiologyFindings",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ChestDistributionId = table.Column<int>(nullable: false),
                    ChestLocationId = table.Column<int>(nullable: false),
                    DateTaken = table.Column<DateTime>(nullable: false),
                    FindingId = table.Column<int>(nullable: false),
                    GradeId = table.Column<int>(nullable: false),
                    Note = table.Column<string>(type: "ntext", nullable: true),
                    PatientId = table.Column<int>(nullable: false),
                    RadiologyTypeId = table.Column<int>(nullable: false),
                    TreatmentResponseId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientRadiologyFindings", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientRadiologyFindings");
        }
    }
}
