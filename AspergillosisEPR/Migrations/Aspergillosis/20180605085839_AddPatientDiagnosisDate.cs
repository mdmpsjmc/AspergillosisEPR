using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPatientDiagnosisDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DiagnosisDate",
                table: "PatientDiagnosis",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PulmonaryFunctionTests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PulmonaryFunctionTests", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PulmonaryFunctionTests");

            migrationBuilder.DropColumn(
                name: "DiagnosisDate",
                table: "PatientDiagnosis");
        }
    }
}
