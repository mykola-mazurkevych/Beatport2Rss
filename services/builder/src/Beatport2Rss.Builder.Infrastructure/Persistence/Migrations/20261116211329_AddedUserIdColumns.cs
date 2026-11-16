using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserIdColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "builder",
                table: "Tags",
                type: "uuid",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "builder",
                table: "Feeds",
                type: "uuid",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "builder",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "builder",
                table: "Feeds");
        }
    }
}
