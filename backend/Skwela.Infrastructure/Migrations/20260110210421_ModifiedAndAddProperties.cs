using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skwela.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAndAddProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Users",
                newName: "user_id");

            migrationBuilder.AddColumn<string>(
                name: "display_image",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_name",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "user_created_at",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    class_description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    class_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
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

            migrationBuilder.CreateIndex(
                name: "IX_Users_role",
                table: "Users",
                column: "role");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_class_id",
                table: "Classrooms",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_user_id",
                table: "Classrooms",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropIndex(
                name: "IX_Users_role",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "display_image",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "display_name",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "user_created_at",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "Users",
                newName: "id");
        }
    }
}
