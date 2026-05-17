using System;
using esecai.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esecai.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260518020000_ManualReleaseNotifications")]
    public partial class ManualReleaseNotifications : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Releases",
                columns: table => new
                {
                    release_id = table.Column<Guid>(type: "uuid", nullable: false),
                    release_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    release_title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    release_summary = table.Column<string>(type: "character varying(400)", maxLength: 400, nullable: false),
                    release_body = table.Column<string>(type: "text", nullable: false),
                    release_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "draft"),
                    release_published_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    release_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    release_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    published_by_user_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Releases", x => x.release_id);
                    table.ForeignKey(
                        name: "FK_Releases_Users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Releases_Users_published_by_user_id",
                        column: x => x.published_by_user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserReleaseReads",
                columns: table => new
                {
                    user_release_read_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    release_id = table.Column<Guid>(type: "uuid", nullable: false),
                    read_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReleaseReads", x => x.user_release_read_id);
                    table.ForeignKey(
                        name: "FK_UserReleaseReads_Releases_release_id",
                        column: x => x.release_id,
                        principalTable: "Releases",
                        principalColumn: "release_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserReleaseReads_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Releases_created_by_user_id",
                table: "Releases",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_published_by_user_id",
                table: "Releases",
                column: "published_by_user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_release_published_at",
                table: "Releases",
                column: "release_published_at");

            migrationBuilder.CreateIndex(
                name: "IX_Releases_release_status",
                table: "Releases",
                column: "release_status");

            migrationBuilder.CreateIndex(
                name: "IX_UserReleaseReads_release_id",
                table: "UserReleaseReads",
                column: "release_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserReleaseReads_user_id_release_id",
                table: "UserReleaseReads",
                columns: new[] { "user_id", "release_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserReleaseReads");

            migrationBuilder.DropTable(
                name: "Releases");
        }
    }
}
