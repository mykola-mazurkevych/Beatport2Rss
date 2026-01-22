using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Beatport2Rss.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenamedDateProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "SubscriptionTags");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "FeedSubscriptions");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Tracks",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ExpirationDate",
                table: "Tokens",
                newName: "ExpiresAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Tokens",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Tags",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "PulledDate",
                table: "Subscriptions",
                newName: "RefreshedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Subscriptions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Releases",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Feeds",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "SubscriptionTags",
                type: "timestamp with time zone",
                nullable: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedAt",
                table: "FeedSubscriptions",
                type: "timestamp with time zone",
                nullable: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SubscriptionTags");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "FeedSubscriptions");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Tracks",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "ExpiresAt",
                table: "Tokens",
                newName: "ExpirationDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Tokens",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Tags",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "RefreshedAt",
                table: "Subscriptions",
                newName: "PulledDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Subscriptions",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Releases",
                newName: "CreatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Feeds",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "SubscriptionTags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "current_timestamp");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedDate",
                table: "FeedSubscriptions",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "current_timestamp");
        }
    }
}
