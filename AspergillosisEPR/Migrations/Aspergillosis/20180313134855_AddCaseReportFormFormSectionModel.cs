using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddCaseReportFormFormSectionModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropIndex(
                name: "IX_CaseReportFormSections_CaseReportFormID",
                table: "CaseReportFormSections");

            migrationBuilder.DropColumn(
                name: "CaseReportFormID",
                table: "CaseReportFormSections");

            migrationBuilder.RenameColumn(
                name: "CaseReportFormID",
                table: "CaseReportFormFields",
                newName: "CaseReportFormId");

            migrationBuilder.RenameIndex(
                name: "IX_CaseReportFormFields_CaseReportFormID",
                table: "CaseReportFormFields",
                newName: "IX_CaseReportFormFields_CaseReportFormId");

            migrationBuilder.AlterColumn<int>(
                name: "CaseReportFormSectionId",
                table: "CaseReportFormFields",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "CaseReportFormFormSections",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CaseReportFormId = table.Column<int>(nullable: false),
                    CaseReportFormSectionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseReportFormFormSections", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CaseReportFormFormSections_CaseReportForms_CaseReportFormId",
                        column: x => x.CaseReportFormId,
                        principalTable: "CaseReportForms",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormFormSections_CaseReportFormId",
                table: "CaseReportFormFormSections",
                column: "CaseReportFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormFields_CaseReportForms_CaseReportFormId",
                table: "CaseReportFormFields",
                column: "CaseReportFormId",
                principalTable: "CaseReportForms",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionId",
                table: "CaseReportFormFields",
                column: "CaseReportFormSectionId",
                principalTable: "CaseReportFormSections",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormFields_CaseReportForms_CaseReportFormId",
                table: "CaseReportFormFields");

            migrationBuilder.DropForeignKey(
                name: "FK_CaseReportFormFields_CaseReportFormSections_CaseReportFormSectionId",
                table: "CaseReportFormFields");

            migrationBuilder.DropTable(
                name: "CaseReportFormFormSections");

            migrationBuilder.RenameColumn(
                name: "CaseReportFormId",
                table: "CaseReportFormFields",
                newName: "CaseReportFormID");

            migrationBuilder.RenameIndex(
                name: "IX_CaseReportFormFields_CaseReportFormId",
                table: "CaseReportFormFields",
                newName: "IX_CaseReportFormFields_CaseReportFormID");

            migrationBuilder.AddColumn<int>(
                name: "CaseReportFormID",
                table: "CaseReportFormSections",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CaseReportFormSectionId",
                table: "CaseReportFormFields",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseReportFormSections_CaseReportFormID",
                table: "CaseReportFormSections",
                column: "CaseReportFormID");

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
    }
}
