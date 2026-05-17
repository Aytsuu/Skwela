using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace esecai.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialEntities : Migration
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
                    class_banner = table.Column<string>(type: "text", nullable: false),
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
                name: "Notifications",
                columns: table => new
                {
                    notif_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notif_title = table.Column<string>(type: "text", nullable: false),
                    notif_message = table.Column<string>(type: "text", nullable: false),
                    notif_is_read = table.Column<bool>(type: "boolean", nullable: false),
                    notif_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.notif_id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    student_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_fname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    student_mname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    student_lname = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    student_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    student_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    class_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.student_id);
                    table.ForeignKey(
                        name: "FK_Students_Classrooms_class_id",
                        column: x => x.class_id,
                        principalTable: "Classrooms",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assessments",
                columns: table => new
                {
                    ass_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ass_title = table.Column<string>(type: "text", nullable: false),
                    ass_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ass_answer_key_url = table.Column<string>(type: "text", nullable: false),
                    ass_instruction = table.Column<string>(type: "text", nullable: false),
                    ass_total_points = table.Column<float>(type: "real", nullable: false),
                    ass_status = table.Column<string>(type: "text", nullable: false, defaultValue: "draft"),
                    ass_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ass_updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    Questionquest_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assessments", x => x.ass_id);
                    table.ForeignKey(
                        name: "FK_Assessments_Classrooms_class_id",
                        column: x => x.class_id,
                        principalTable: "Classrooms",
                        principalColumn: "class_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    quest_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quest_num = table.Column<int>(type: "integer", nullable: false),
                    quest_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quest_text = table.Column<string>(type: "text", nullable: false),
                    quest_correct_answer = table.Column<string>(type: "jsonb", nullable: false),
                    quest_rubric = table.Column<string>(type: "jsonb", nullable: false),
                    quest_max_points = table.Column<float>(type: "real", nullable: false),
                    quest_ai_confidence = table.Column<float>(type: "real", nullable: false),
                    ass_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.quest_id);
                    table.ForeignKey(
                        name: "FK_Questions_Assessments_ass_id",
                        column: x => x.ass_id,
                        principalTable: "Assessments",
                        principalColumn: "ass_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    rec_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rec_scan_url = table.Column<string>(type: "text", nullable: false),
                    rec_total_score = table.Column<float>(type: "real", nullable: false),
                    rec_percentage = table.Column<float>(type: "real", nullable: false),
                    rec_status = table.Column<string>(type: "text", nullable: false, defaultValue: "pending"),
                    rec_graded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    rec_created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ass_id = table.Column<Guid>(type: "uuid", nullable: false),
                    student_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => x.rec_id);
                    table.ForeignKey(
                        name: "FK_Records_Assessments_ass_id",
                        column: x => x.ass_id,
                        principalTable: "Assessments",
                        principalColumn: "ass_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Records_Students_student_id",
                        column: x => x.student_id,
                        principalTable: "Students",
                        principalColumn: "student_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordAnswers",
                columns: table => new
                {
                    ra_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ra_student_ans = table.Column<string>(type: "jsonb", nullable: false),
                    ra_awarded_pts = table.Column<float>(type: "real", nullable: false),
                    ra_ai_confidence = table.Column<float>(type: "real", nullable: false),
                    ra_feedback = table.Column<string>(type: "text", nullable: false),
                    ra_teacher_rev = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    ra_teacher_op = table.Column<float>(type: "real", nullable: false),
                    ra_raw_response = table.Column<string>(type: "jsonb", nullable: false),
                    quest_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rec_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordAnswers", x => x.ra_id);
                    table.ForeignKey(
                        name: "FK_RecordAnswers_Questions_quest_id",
                        column: x => x.quest_id,
                        principalTable: "Questions",
                        principalColumn: "quest_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RecordAnswers_Records_rec_id",
                        column: x => x.rec_id,
                        principalTable: "Records",
                        principalColumn: "rec_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_ass_id",
                table: "Assessments",
                column: "ass_id");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_class_id",
                table: "Assessments",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_Questionquest_id",
                table: "Assessments",
                column: "Questionquest_id");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_class_id",
                table: "Classrooms",
                column: "class_id");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_user_id",
                table: "Classrooms",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_user_id",
                table: "Notifications",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_ass_id",
                table: "Questions",
                column: "ass_id");

            migrationBuilder.CreateIndex(
                name: "IX_RecordAnswers_quest_id",
                table: "RecordAnswers",
                column: "quest_id");

            migrationBuilder.CreateIndex(
                name: "IX_RecordAnswers_rec_id",
                table: "RecordAnswers",
                column: "rec_id");

            migrationBuilder.CreateIndex(
                name: "IX_Records_ass_id",
                table: "Records",
                column: "ass_id");

            migrationBuilder.CreateIndex(
                name: "IX_Records_student_id",
                table: "Records",
                column: "student_id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_class_id_student_fname_student_mname_student_lname",
                table: "Students",
                columns: new[] { "class_id", "student_fname", "student_mname", "student_lname" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Questions_Questionquest_id",
                table: "Assessments",
                column: "Questionquest_id",
                principalTable: "Questions",
                principalColumn: "quest_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Classrooms_class_id",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Questions_Questionquest_id",
                table: "Assessments");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RecordAnswers");

            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Assessments");
        }
    }
}
