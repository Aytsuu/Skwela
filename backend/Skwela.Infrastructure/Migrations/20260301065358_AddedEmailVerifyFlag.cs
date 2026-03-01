using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skwela.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedEmailVerifyFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_email_verified",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_email_verified",
                table: "Users");
        }
    }
}
