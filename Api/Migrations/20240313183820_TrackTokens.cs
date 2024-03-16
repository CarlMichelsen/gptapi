using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class TrackTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResponseId",
                schema: "GptApi",
                table: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "GptApi",
                table: "Message",
                type: "text",
                nullable: false,
                defaultValue: string.Empty,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsageId",
                schema: "GptApi",
                table: "Message",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Usage",
                schema: "GptApi",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Provider = table.Column<int>(type: "integer", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: false),
                    Tokens = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usage", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Message_UsageId",
                schema: "GptApi",
                table: "Message",
                column: "UsageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Usage_UsageId",
                schema: "GptApi",
                table: "Message",
                column: "UsageId",
                principalSchema: "GptApi",
                principalTable: "Usage",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Usage_UsageId",
                schema: "GptApi",
                table: "Message");

            migrationBuilder.DropTable(
                name: "Usage",
                schema: "GptApi");

            migrationBuilder.DropIndex(
                name: "IX_Message_UsageId",
                schema: "GptApi",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "UsageId",
                schema: "GptApi",
                table: "Message");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                schema: "GptApi",
                table: "Message",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ResponseId",
                schema: "GptApi",
                table: "Message",
                type: "text",
                nullable: true);
        }
    }
}
