using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Postfy.Services.Network.Shared.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial003 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "email",
                schema: "network",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                schema: "network",
                table: "users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "email",
                schema: "network",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phone_number",
                schema: "network",
                table: "users");
        }
    }
}
