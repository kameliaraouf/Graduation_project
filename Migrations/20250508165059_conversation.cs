using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class conversation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatBotHistories_Conversation_ConversationId",
                table: "chatBotHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_chatBotHistories_users_UserID",
                table: "chatBotHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_Conversation_users_UserId",
                table: "Conversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_chatBotHistories",
                table: "chatBotHistories");

            migrationBuilder.RenameTable(
                name: "Conversation",
                newName: "Conversations");

            migrationBuilder.RenameTable(
                name: "chatBotHistories",
                newName: "ChatBotHistory");

            migrationBuilder.RenameIndex(
                name: "IX_Conversation_UserId",
                table: "Conversations",
                newName: "IX_Conversations_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_chatBotHistories_UserID",
                table: "ChatBotHistory",
                newName: "IX_ChatBotHistory_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_chatBotHistories_ConversationId",
                table: "ChatBotHistory",
                newName: "IX_ChatBotHistory_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChatBotHistory",
                table: "ChatBotHistory",
                column: "Id");

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
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversations_users_UserId",
                table: "Conversations",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
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
                name: "FK_Conversations_users_UserId",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Conversations",
                table: "Conversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChatBotHistory",
                table: "ChatBotHistory");

            migrationBuilder.RenameTable(
                name: "Conversations",
                newName: "Conversation");

            migrationBuilder.RenameTable(
                name: "ChatBotHistory",
                newName: "chatBotHistories");

            migrationBuilder.RenameIndex(
                name: "IX_Conversations_UserId",
                table: "Conversation",
                newName: "IX_Conversation_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ChatBotHistory_UserID",
                table: "chatBotHistories",
                newName: "IX_chatBotHistories_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_ChatBotHistory_ConversationId",
                table: "chatBotHistories",
                newName: "IX_chatBotHistories_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Conversation",
                table: "Conversation",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_chatBotHistories",
                table: "chatBotHistories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_chatBotHistories_Conversation_ConversationId",
                table: "chatBotHistories",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_chatBotHistories_users_UserID",
                table: "chatBotHistories",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Conversation_users_UserId",
                table: "Conversation",
                column: "UserId",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
