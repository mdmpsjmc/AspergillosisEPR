using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AspergillosisEPR.Migrations.Aspergillosis
{
    public partial class AddPostCodes : Migration
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
                    Latitude = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKOutwardCodes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "UKPostCodes",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(9,4)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(9,4)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UKPostCodes", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UKOutwardCodes");

            migrationBuilder.DropTable(
                name: "UKPostCodes");

            migrationBuilder.DropColumn(
                name: "DistanceFromWythenshawe",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "PostCode",
                table: "Patients");
        }
    }
}
