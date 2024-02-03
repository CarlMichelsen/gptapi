using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "GptApi");

            migrationBuilder.CreateTable(
                name: "Conversation",
                schema: "GptApi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uuid", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAppendedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserArchived = table.Column<bool>(type: "boolean", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                schema: "GptApi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousMessageId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResponseId = table.Column<string>(type: "text", nullable: true),
                    Visible = table.Column<bool>(type: "boolean", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ConversationId = table.Column<Guid>(type: "uuid", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalSchema: "GptApi",
                        principalTable: "Conversation",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Message_Message_PreviousMessageId",
                        column: x => x.PreviousMessageId,
                        principalSchema: "GptApi",
                        principalTable: "Message",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationId",
                schema: "GptApi",
                table: "Message",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_PreviousMessageId",
                schema: "GptApi",
                table: "Message",
                column: "PreviousMessageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Message",
                schema: "GptApi");

            migrationBuilder.DropTable(
                name: "Conversation",
                schema: "GptApi");
        }
    }
}
