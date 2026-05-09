using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SAP_AdresToLatLong.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SAPData",
                columns: table => new
                {
                    DocNum = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CardCode = table.Column<string>(type: "text", nullable: false),
                    BillToAddress = table.Column<string>(type: "text", nullable: false),
                    SendToAddress = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SAPData", x => x.DocNum);
                });

            migrationBuilder.CreateTable(
                name: "PostGeocodeData",
                columns: table => new
                {
                    DocNum = table.Column<int>(type: "integer", nullable: false),
                    CardCode = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(9,6)", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(10,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostGeocodeData", x => x.DocNum);
                    table.ForeignKey(
                        name: "FK_PostGeocodeData_SAPData_DocNum",
                        column: x => x.DocNum,
                        principalTable: "SAPData",
                        principalColumn: "DocNum",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostGeocodeData");

            migrationBuilder.DropTable(
                name: "SAPData");
        }
    }
}
