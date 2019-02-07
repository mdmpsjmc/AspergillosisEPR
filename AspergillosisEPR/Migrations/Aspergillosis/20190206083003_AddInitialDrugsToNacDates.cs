using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddInitialDrugsToNacDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       
            migrationBuilder.AddColumn<string>(
                name: "FollowUp3MonthsDrug",
                table: "PatientNACDates",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InitialDrug",
                table: "PatientNACDates",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ReferralDate",
                table: "PatientNACDates",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.DropColumn(
                name: "FollowUp3MonthsDrug",
                table: "PatientNACDates");

            migrationBuilder.DropColumn(
                name: "InitialDrug",
                table: "PatientNACDates");

            migrationBuilder.DropColumn(
                name: "ReferralDate",
                table: "PatientNACDates");

      
        }
    }
}
