using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GraduationProject.Migrations
{
    /// <inheritdoc />
    public partial class @try : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Review",
                table: "products");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ProductReviews",
                newName: "ReviewDate");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "ProductReviews",
                newName: "ReviewText");

            migrationBuilder.RenameColumn(
                name: "Qustion",
                table: "chatBotHistories",
                newName: "Question");

            migrationBuilder.AlterColumn<double>(
                name: "AverageRating",
                table: "products",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "ProductReviews",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ConfirmationCode",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "chatBotHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversation_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_chatBotHistories_ConversationId",
                table: "chatBotHistories",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_UserId",
                table: "Conversation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_chatBotHistories_Conversation_ConversationId",
                table: "chatBotHistories",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_chatBotHistories_Conversation_ConversationId",
                table: "chatBotHistories");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropIndex(
                name: "IX_chatBotHistories_ConversationId",
                table: "chatBotHistories");

            migrationBuilder.DropColumn(
                name: "ConfirmationCode",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "chatBotHistories");

            migrationBuilder.RenameColumn(
                name: "ReviewText",
                table: "ProductReviews",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "ReviewDate",
                table: "ProductReviews",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "Question",
                table: "chatBotHistories",
                newName: "Qustion");

            migrationBuilder.AlterColumn<double>(
                name: "AverageRating",
                table: "products",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Review",
                table: "products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Rating",
                table: "ProductReviews",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
