using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class conversation3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotHistory_Conversations_ConversationId",
                table: "ChatBotHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotHistory_users_UserID",
                table: "ChatBotHistory");

            migrationBuilder.AddColumn<int>(
                name: "ChatHistoryId",
                table: "Conversations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ChatHistoryId",
                table: "Conversations",
                column: "ChatHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotHistory_Conversations_ConversationId",
                table: "ChatBotHistory",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotHistory_users_UserID",
                table: "ChatBotHistory",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_ChatBotHistory_ChatHistoryId",
                table: "Conversations",
                column: "ChatHistoryId",
                principalTable: "ChatBotHistory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotHistory_Conversations_ConversationId",
                table: "ChatBotHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatBotHistory_users_UserID",
                table: "ChatBotHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversations_ChatBotHistory_ChatHistoryId",
                table: "Conversations");

            migrationBuilder.DropIndex(
                name: "IX_Conversations_ChatHistoryId",
                table: "Conversations");

            migrationBuilder.DropColumn(
                name: "ChatHistoryId",
                table: "Conversations");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotHistory_Conversations_ConversationId",
                table: "ChatBotHistory",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatBotHistory_users_UserID",
                table: "ChatBotHistory",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
