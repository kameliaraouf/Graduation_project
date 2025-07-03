using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class conversation2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotHistory_Conversations_ConversationId",
                table: "ChatBotHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_users_UserId",
                table: "Conversations");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotHistory_Conversations_ConversationId",
                table: "ChatBotHistory",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_users_UserId",
                table: "Conversations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotHistory_Conversations_ConversationId",
                table: "ChatBotHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_users_UserId",
                table: "Conversations");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotHistory_Conversations_ConversationId",
                table: "ChatBotHistory",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_users_UserId",
                table: "Conversations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
