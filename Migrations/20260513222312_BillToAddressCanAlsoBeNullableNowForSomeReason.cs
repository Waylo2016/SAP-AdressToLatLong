using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SAP_AdresToLatLong.Migrations
{
    /// <inheritdoc />
    public partial class BillToAddressCanAlsoBeNullableNowForSomeReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BillToAddress",
                table: "SAPData",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BillToAddress",
                table: "SAPData",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
