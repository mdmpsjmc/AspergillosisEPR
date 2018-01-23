using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class FixImmunoglobulinTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientImmunoglobulins_ImmunoglobulinTypes_ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins");

            migrationBuilder.DropColumn(
                name: "ImmunoglobinTypeID",
                table: "PatientImmunoglobulins");

            migrationBuilder.RenameColumn(
                name: "ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins",
                newName: "ImmunoglobulinTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_PatientImmunoglobulins_ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins",
                newName: "IX_PatientImmunoglobulins_ImmunoglobulinTypeId");

            migrationBuilder.AlterColumn<int>(
                name: "ImmunoglobulinTypeId",
                table: "PatientImmunoglobulins",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientImmunoglobulins_ImmunoglobulinTypes_ImmunoglobulinTypeId",
                table: "PatientImmunoglobulins",
                column: "ImmunoglobulinTypeId",
                principalTable: "ImmunoglobulinTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PatientImmunoglobulins_ImmunoglobulinTypes_ImmunoglobulinTypeId",
                table: "PatientImmunoglobulins");

            migrationBuilder.RenameColumn(
                name: "ImmunoglobulinTypeId",
                table: "PatientImmunoglobulins",
                newName: "ImmunoglobulinTypeID");

            migrationBuilder.RenameIndex(
                name: "IX_PatientImmunoglobulins_ImmunoglobulinTypeId",
                table: "PatientImmunoglobulins",
                newName: "IX_PatientImmunoglobulins_ImmunoglobulinTypeID");

            migrationBuilder.AlterColumn<int>(
                name: "ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ImmunoglobinTypeID",
                table: "PatientImmunoglobulins",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientImmunoglobulins_ImmunoglobulinTypes_ImmunoglobulinTypeID",
                table: "PatientImmunoglobulins",
                column: "ImmunoglobulinTypeID",
                principalTable: "ImmunoglobulinTypes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
