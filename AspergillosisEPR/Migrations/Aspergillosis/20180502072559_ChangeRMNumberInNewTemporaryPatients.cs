using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class ChangeRMNumberInNewTemporaryPatients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RM2Number",
                table: "TemporaryNewPatients",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RM2Number",
                table: "TemporaryNewPatients",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
