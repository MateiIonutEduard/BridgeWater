using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BridgeWater.Migrations
{
    /// <inheritdoc />
    public partial class BridgeWater_20230510 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Webcode",
                table: "Account",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Webcode",
                table: "Account");
        }
    }
}
