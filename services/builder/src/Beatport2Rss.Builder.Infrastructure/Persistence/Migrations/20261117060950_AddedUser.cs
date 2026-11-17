using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                schema: "builder",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UserId",
                schema: "builder",
                table: "Tags",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_UserId",
                schema: "builder",
                table: "Feeds",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feeds_Users_UserId",
                schema: "builder",
                table: "Feeds",
                column: "UserId",
                principalSchema: "builder",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_UserId",
                schema: "builder",
                table: "Tags",
                column: "UserId",
                principalSchema: "builder",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feeds_Users_UserId",
                schema: "builder",
                table: "Feeds");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_UserId",
                schema: "builder",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "builder");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UserId",
                schema: "builder",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Feeds_UserId",
                schema: "builder",
                table: "Feeds");
        }
    }
}
