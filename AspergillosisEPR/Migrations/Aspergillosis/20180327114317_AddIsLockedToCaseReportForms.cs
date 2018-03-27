using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddIsLockedToCaseReportForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLocked",
                table: "CaseReportForms",
                nullable: false,
                defaultValue: false);
          
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormResults_CaseReportFormCategories_CaseReportFormCategoryId",
                table: "CaseReportFormResults");
        }

    }
}
