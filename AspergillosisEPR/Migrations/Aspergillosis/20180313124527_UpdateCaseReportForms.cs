using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class UpdateCaseReportForms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionID",
                table: "CaseReportFormFields");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormSections_CaseReportFormResults_CaseReportFormResultID",
                table: "CaseReportFormSections");

            migrationBuilder.DropTable(
                name: "CaseReportFormResults");

            migrationBuilder.RenameColumn(
                name: "CaseReportFormResultID",
                table: "CaseReportFormSections",
                newName: "CaseReportFormID");

            migrationBuilder.RenameIndex(
                name: "IX_CaseReportFormSections_CaseReportFormResultID",
                table: "CaseReportFormSections",
                newName: "IX_CaseReportFormSections_CaseReportFormID");

            migrationBuilder.RenameColumn(
                name: "CaseReportFormSectionID",
                table: "CaseReportFormFields",
                newName: "CaseReportFormSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_CaseReportFormFields_CaseReportFormSectionID",
                table: "CaseReportFormFields",
                newName: "IX_CaseReportFormFields_CaseReportFormSectionId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CaseReportFormSections",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "CaseReportFormFields",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CaseReportFormSectionId",
                table: "CaseReportFormFields",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CaseReportFormID",
                table: "CaseReportFormFields",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CaseReportForms",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormCategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportForms", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportForms_CaseReportFormCategories_CaseReportFormCategoryId",
                        column: x => x.CaseReportFormCategoryId,
                        principalTable: "CaseReportFormCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormFields_CaseReportFormID",
                table: "CaseReportFormFields",
                column: "CaseReportFormID");

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportForms_CaseReportFormCategoryId",
                table: "CaseReportForms",
                column: "CaseReportFormCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormFields_CaseReportForms_CaseReportFormID",
                table: "CaseReportFormFields",
                column: "CaseReportFormID",
                principalTable: "CaseReportForms",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionId",
                table: "CaseReportFormFields",
                column: "CaseReportFormSectionId",
                principalTable: "CaseReportFormSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormSections_CaseReportForms_CaseReportFormID",
                table: "CaseReportFormSections",
                column: "CaseReportFormID",
                principalTable: "CaseReportForms",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormFields_CaseReportForms_CaseReportFormID",
                table: "CaseReportFormFields");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionId",
                table: "CaseReportFormFields");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormSections_CaseReportForms_CaseReportFormID",
                table: "CaseReportFormSections");

            migrationBuilder.DropTable(
                name: "CaseReportForms");

            migrationBuilder.DropIndex(
                name: "IX_CaseReportFormFields_CaseReportFormID",
                table: "CaseReportFormFields");

            migrationBuilder.DropColumn(
                name: "CaseReportFormID",
                table: "CaseReportFormFields");

            migrationBuilder.RenameColumn(
                name: "CaseReportFormID",
                table: "CaseReportFormSections",
                newName: "CaseReportFormResultID");

            migrationBuilder.RenameIndex(
                name: "IX_CaseReportFormSections_CaseReportFormID",
                table: "CaseReportFormSections",
                newName: "IX_CaseReportFormSections_CaseReportFormResultID");

            migrationBuilder.RenameColumn(
                name: "CaseReportFormSectionId",
                table: "CaseReportFormFields",
                newName: "CaseReportFormSectionID");

            migrationBuilder.RenameIndex(
                name: "IX_CaseReportFormFields_CaseReportFormSectionId",
                table: "CaseReportFormFields",
                newName: "IX_CaseReportFormFields_CaseReportFormSectionID");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CaseReportFormSections",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Label",
                table: "CaseReportFormFields",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<int>(
                name: "CaseReportFormSectionID",
                table: "CaseReportFormFields",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "CaseReportFormResults",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormCategoryID = table.Column<int>(nullable: true),
                    CaseReportFormResultCategoryId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormResults", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormResults_CaseReportFormCategories_CaseReportFormCategoryID",
                        column: x => x.CaseReportFormCategoryID,
                        principalTable: "CaseReportFormCategories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormResults_CaseReportFormCategoryID",
                table: "CaseReportFormResults",
                column: "CaseReportFormCategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionID",
                table: "CaseReportFormFields",
                column: "CaseReportFormSectionID",
                principalTable: "CaseReportFormSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormSections_CaseReportFormResults_CaseReportFormResultID",
                table: "CaseReportFormSections",
                column: "CaseReportFormResultID",
                principalTable: "CaseReportFormResults",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
