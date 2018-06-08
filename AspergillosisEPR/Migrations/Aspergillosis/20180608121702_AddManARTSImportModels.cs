using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddManARTSImportModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GenericNote",
                table: "Patients",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PatientPulmonaryFunctionTests",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DateTaken = table.Column<DateTime>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    PulmonaryFunctionTestId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientPulmonaryFunctionTests", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "PatientSurgeries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Note = table.Column<string>(nullable: true),
                    PatientId = table.Column<int>(nullable: false),
                    SurgeryDate = table.Column<int>(nullable: true),
                    SurgeryId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientSurgeries", x => x.ID);
                });

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

            migrationBuilder.CreateTable(
                name: "SmokingStatuses",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SmokingStatuses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Surgeries",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Surgeries", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PatientPulmonaryFunctionTests");

            migrationBuilder.DropTable(
                name: "PatientSurgeries");

            migrationBuilder.DropTable(
                name: "PulmonaryFunctionTests");

            migrationBuilder.DropTable(
                name: "SmokingStatuses");

            migrationBuilder.DropTable(
                name: "Surgeries");

            migrationBuilder.DropColumn(
                name: "GenericNote",
                table: "Patients");
        }
    }
}
