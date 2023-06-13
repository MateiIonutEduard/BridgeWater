using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BridgeWater.Migrations
{
    /// <inheritdoc />
    public partial class ReplyToMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReplyTo",
                table: "Post",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyTo",
                table: "Post");
        }
    }
}
