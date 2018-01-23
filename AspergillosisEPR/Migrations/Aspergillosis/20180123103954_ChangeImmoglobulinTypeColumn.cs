using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangeImmoglobulinTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImmunoglobinTypeId",
                table: "PatientImmunoglobulins",
                newName: "ImmunoglobinTypeID");

            migrationBuilder.AddColumn<int>(
                name: "ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PatientImmunoglobulins_ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins",
                column: "ImmunoglobulinTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_PatientImmunoglobulins_PatientId",
                table: "PatientImmunoglobulins",
                column: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_PatientImmunoglobulins_ImmunoglobulinTypes_ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins",
                column: "ImmunoglobulinTypeID",
                principalTable: "ImmunoglobulinTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientImmunoglobulins_Patients_PatientId",
                table: "PatientImmunoglobulins",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientImmunoglobulins_ImmunoglobulinTypes_ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientImmunoglobulins_Patients_PatientId",
                table: "PatientImmunoglobulins");

            migrationBuilder.DropIndex(
                name: "IX_PatientImmunoglobulins_ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins");

            migrationBuilder.DropIndex(
                name: "IX_PatientImmunoglobulins_PatientId",
                table: "PatientImmunoglobulins");

            migrationBuilder.DropColumn(
                name: "ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins");

            migrationBuilder.RenameColumn(
                name: "ImmunoglobinTypeID",
                table: "PatientImmunoglobulins",
                newName: "ImmunoglobinTypeId");
        }
    }
}
