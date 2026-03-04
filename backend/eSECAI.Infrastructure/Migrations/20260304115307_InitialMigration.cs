using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eSECAI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    display_image = table.Column<string>(type: "text", nullable: false),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    user_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    refreshToken = table.Column<string>(type: "text", nullable: true),
                    refreshTokenExpiryTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                });

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    class_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    class_image = table.Column<string>(type: "text", nullable: false),
                    class_is_archived = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    class_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    class_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.class_id);
                    table.ForeignKey(
                        name: "FK_Classrooms_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    enroll_id = table.Column<Guid>(type: "uuid", nullable: false),
                    enroll_is_approved = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    enroll_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.enroll_id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Classrooms_class_id",
                        column: x => x.class_id,
                        principalTable: "Classrooms",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Postings",
                columns: table => new
                {
                    post_id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_heading = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    post_details = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    post_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    post_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    class_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Postings", x => x.post_id);
                    table.ForeignKey(
                        name: "FK_Postings_Classrooms_class_id",
                        column: x => x.class_id,
                        principalTable: "Classrooms",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    sub_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sub_score = table.Column<float>(type: "real", nullable: false, defaultValue: 0f),
                    sub_remark = table.Column<string>(type: "text", nullable: false),
                    sub_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    post_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.sub_id);
                    table.ForeignKey(
                        name: "FK_Submissions_Postings_post_id",
                        column: x => x.post_id,
                        principalTable: "Postings",
                        principalColumn: "post_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Submissions_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_class_id",
                table: "Classrooms",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_user_id",
                table: "Classrooms",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_class_id",
                table: "Enrollments",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_user_id",
                table: "Enrollments",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Postings_class_id",
                table: "Postings",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_post_id",
                table: "Submissions",
                column: "post_id");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_user_id",
                table: "Submissions",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Postings");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
