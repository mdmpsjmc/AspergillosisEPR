using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.ExternalImportDb
{
    public partial class CreatePatients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Patients",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LastName = table.Column<string>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    Gender = table.Column<string>(nullable: true),
                    DOB = table.Column<DateTime>(nullable: false),
                    RM2Number = table.Column<string>(maxLength: 50, nullable: false),
                    PatientStatusId = table.Column<int>(nullable: true),
                    DateOfDeath = table.Column<DateTime>(nullable: true),
                    NhsNumber = table.Column<string>(nullable: true),
                    GenericNote = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    DistanceFromWythenshawe = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Patients", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Patients_PatientStatus_PatientStatusId",
                        column: x => x.PatientStatusId,
                        principalTable: "PatientStatus",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });
        


            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientStatusId",
                table: "Patients",
                column: "PatientStatusId");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {          

            migrationBuilder.DropTable(
                name: "Patients");

          
        }
    }
}
