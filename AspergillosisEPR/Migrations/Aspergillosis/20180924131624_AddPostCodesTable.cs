using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPostCodesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DistanceFromWythenshawe",
                table: "Patients",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PostCode",
                table: "Patients",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UKOutwardCodes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Latitude = table.Column<decimal>(nullable: false),
                    Longitude = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKOutwardCodes", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UKOutwardCodes");

            migrationBuilder.DropColumn(
                name: "DistanceFromWythenshawe",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Patients");
        }
    }
}
