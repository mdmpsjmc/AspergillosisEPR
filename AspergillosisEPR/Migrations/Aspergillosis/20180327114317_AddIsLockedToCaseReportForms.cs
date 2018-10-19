using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddIsLockedToCaseReportForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
                     
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormResults_CaseReportFormCategories_CaseReportFormCategoryId",
                table: "CaseReportFormResults");
        }

    }
}
