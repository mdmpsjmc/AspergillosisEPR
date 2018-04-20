using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddUnitOfMeasuresAddDrugLevels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PatientDrugLevel",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ComparisionCharacter = table.Column<string>(nullable: true),
                    DateReceived = table.Column<DateTime>(nullable: false),
                    DateTaken = table.Column<DateTime>(nullable: false),
                    DrugId = table.Column<int>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    ResultValue = table.Column<decimal>(nullable: false),
                    UnitOfMeasurementId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientDrugLevel", x => x.ID);                  
                    table.ForeignKey(
                        name: "FK_PatientDrugLevel_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UnitOfMeasurements",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitOfMeasurements", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientDrugLevel_DrugId",
                table: "PatientDrugLevel",
                column: "DrugId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientDrugLevel_PatientId",
                table: "PatientDrugLevel",
                column: "PatientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientDrugLevel");

            migrationBuilder.DropTable(
                name: "UnitOfMeasurements");
        }
    }
}
