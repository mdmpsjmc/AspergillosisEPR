using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddRadiologyOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RadiologyFindingSelectOptions",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyFindingSelectOptions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyFindingSelects",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsMultiple = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyFindingSelects", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "RadiologyResults",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RadiologyFindingSelectId = table.Column<int>(nullable: false),
                    RadiologyFindingSelectOptionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_RadiologyResults_RadiologyFindingSelects_RadiologyFindingSelectId",
                        column: x => x.RadiologyFindingSelectId,
                        principalTable: "RadiologyFindingSelects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RadiologyResults_RadiologyFindingSelectOptions_RadiologyFindingSelectOptionId",
                        column: x => x.RadiologyFindingSelectOptionId,
                        principalTable: "RadiologyFindingSelectOptions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_RadiologyFindingSelectId",
                table: "RadiologyResults",
                column: "RadiologyFindingSelectId");

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_RadiologyFindingSelectOptionId",
                table: "RadiologyResults",
                column: "RadiologyFindingSelectOptionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RadiologyResults");

            migrationBuilder.DropTable(
                name: "RadiologyFindingSelects");

            migrationBuilder.DropTable(
                name: "RadiologyFindingSelectOptions");
        }
    }
}
