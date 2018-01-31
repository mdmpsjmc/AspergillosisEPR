using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangeRadiologyFindingSelect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMultiple",
                table: "RadiologyFindingSelects");

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiple",
                table: "RadiologyResults",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMultiple",
                table: "RadiologyResults");

            migrationBuilder.AddColumn<bool>(
                name: "IsMultiple",
                table: "RadiologyFindingSelects",
                nullable: false,
                defaultValue: false);
        }
    }
}
