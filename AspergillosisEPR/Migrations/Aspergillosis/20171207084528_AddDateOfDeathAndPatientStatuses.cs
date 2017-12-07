using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddDateOfDeathAndPatientStatuses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfDeath",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientStatusId",
                table: "Patients",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientStatuses", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientStatuses");

            migrationBuilder.DropColumn(
                name: "DateOfDeath",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PatientStatusId",
                table: "Patients");
        }
    }
}
