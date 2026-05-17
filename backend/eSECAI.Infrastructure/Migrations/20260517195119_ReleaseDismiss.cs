using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esecai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReleaseDismiss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserReleaseDismisses",
                columns: table => new
                {
                    user_release_dismiss_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    release_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dismissed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReleaseDismisses", x => x.user_release_dismiss_id);
                    table.ForeignKey(
                        name: "FK_UserReleaseDismisses_Releases_release_id",
                        column: x => x.release_id,
                        principalTable: "Releases",
                        principalColumn: "release_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReleaseDismisses_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserReleaseDismisses_release_id",
                table: "UserReleaseDismisses",
                column: "release_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserReleaseDismisses_user_id_release_id",
                table: "UserReleaseDismisses",
                columns: new[] { "user_id", "release_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserReleaseDismisses");
        }
    }
}
