using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddAllergicSideEffects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "IntoleranceType",
                table: "PatientAllergicIntoleranceItems",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "PatientAllergicIntoleranceItemSideEffects",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PatientAllergicIntoleranceItemId = table.Column<int>(nullable: false),
                    SideEffectId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAllergicIntoleranceItemSideEffects", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PatientAllergicIntoleranceItemSideEffects_PatientAllergicIntoleranceItems_PatientAllergicIntoleranceItemId",
                        column: x => x.PatientAllergicIntoleranceItemId,
                        principalTable: "PatientAllergicIntoleranceItems",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PatientAllergicIntoleranceItemSideEffects_SideEffects_SideEffectId",
                        column: x => x.SideEffectId,
                        principalTable: "SideEffects",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergicIntoleranceItems_PatientId",
                table: "PatientAllergicIntoleranceItems",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergicIntoleranceItemSideEffects_PatientAllergicIntoleranceItemId",
                table: "PatientAllergicIntoleranceItemSideEffects",
                column: "PatientAllergicIntoleranceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAllergicIntoleranceItemSideEffects_SideEffectId",
                table: "PatientAllergicIntoleranceItemSideEffects",
                column: "SideEffectId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientAllergicIntoleranceItems_Patients_PatientId",
                table: "PatientAllergicIntoleranceItems",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientAllergicIntoleranceItems_Patients_PatientId",
                table: "PatientAllergicIntoleranceItems");

            migrationBuilder.DropTable(
                name: "PatientAllergicIntoleranceItemSideEffects");

            migrationBuilder.DropIndex(
                name: "IX_PatientAllergicIntoleranceItems_PatientId",
                table: "PatientAllergicIntoleranceItems");

            migrationBuilder.AlterColumn<string>(
                name: "IntoleranceType",
                table: "PatientAllergicIntoleranceItems",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
