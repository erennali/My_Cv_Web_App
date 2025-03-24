using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErenAliKocaCV.Migrations
{
    /// <inheritdoc />
    public partial class added2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomCSS",
                table: "SiteSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomJS",
                table: "SiteSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EnableSEO",
                table: "SiteSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Favicon",
                table: "SiteSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleAnalyticsId",
                table: "SiteSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoogleVerificationId",
                table: "SiteSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ShowContactForm",
                table: "SiteSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowGitHubProjects",
                table: "SiteSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowMediumArticles",
                table: "SiteSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SiteDescription",
                table: "SiteSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteKeywords",
                table: "SiteSettings",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SiteTitle",
                table: "SiteSettings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 3, 23, 23, 2, 52, 916, DateTimeKind.Utc).AddTicks(8730));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomCSS",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "CustomJS",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "EnableSEO",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "Favicon",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "GoogleAnalyticsId",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "GoogleVerificationId",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ShowContactForm",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ShowGitHubProjects",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "ShowMediumArticles",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "SiteDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "SiteKeywords",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "SiteTitle",
                table: "SiteSettings");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 3, 23, 22, 8, 21, 125, DateTimeKind.Utc).AddTicks(2570));
        }
    }
}
