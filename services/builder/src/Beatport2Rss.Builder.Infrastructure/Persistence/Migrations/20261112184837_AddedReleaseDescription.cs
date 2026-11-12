using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Builder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddedReleaseDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "builder",
                table: "Releases",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                schema: "builder",
                table: "Releases");
        }
    }
}
