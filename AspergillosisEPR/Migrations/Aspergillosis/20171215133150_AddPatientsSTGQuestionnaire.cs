using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPatientsSTGQuestionnaire : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientSTGQuestionnaires",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActivityScore = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ImpactScore = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    SymptomScore = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TotalScore = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSTGQuestionnaires", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientSTGQuestionnaires");
        }
    }
}
