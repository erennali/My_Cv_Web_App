using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ErenAliKocaCV.Migrations
{
    /// <inheritdoc />
    public partial class AddFooterSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FooterLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IconClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterLinks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FooterSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AboutTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AboutText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GitHubUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LinkedInUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MediumUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LinksTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ContactTitle = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CopyrightText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FooterSettings", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 5, 18, 14, 17, 38, 745, DateTimeKind.Utc).AddTicks(5540));

            migrationBuilder.InsertData(
                table: "FooterLinks",
                columns: new[] { "Id", "CreatedAt", "IconClass", "Order", "Text", "UpdatedAt", "Url" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5650), "icon-long-arrow-right", 1, "Home", new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5650), "#home-section" },
                    { 2, new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5650), "icon-long-arrow-right", 2, "About", new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5650), "#about" },
                    { 3, new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5660), "icon-long-arrow-right", 3, "GitHub", new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5660), "#github-section" },
                    { 4, new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5660), "icon-long-arrow-right", 4, "Projects", new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5660), "#projects" },
                    { 5, new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5660), "icon-long-arrow-right", 5, "Contact", new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5660), "#contact" }
                });

            migrationBuilder.InsertData(
                table: "FooterSettings",
                columns: new[] { "Id", "AboutText", "AboutTitle", "ContactTitle", "CopyrightText", "CreatedAt", "Email", "GitHubUrl", "LinkedInUrl", "LinksTitle", "MediumUrl", "UpdatedAt" },
                values: new object[] { 1, "Passionate software developer with expertise in creating efficient, scalable, and user-friendly applications across various platforms and technologies.", "About Me", "Have a Question?", "All Rights Reserved | Designed with ❤", new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5560), "eren_ali_koca@hotmail.com", "https://github.com/erennali", "https://www.linkedin.com/in/erenalikoca/", "Site Links", "https://medium.com/@erenali", new DateTime(2025, 5, 18, 17, 17, 38, 745, DateTimeKind.Local).AddTicks(5620) });

            migrationBuilder.CreateIndex(
                name: "IX_FooterLinks_Order",
                table: "FooterLinks",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_FooterLinks_UpdatedAt",
                table: "FooterLinks",
                column: "UpdatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FooterSettings_UpdatedAt",
                table: "FooterSettings",
                column: "UpdatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FooterLinks");

            migrationBuilder.DropTable(
                name: "FooterSettings");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 5, 17, 21, 59, 25, 718, DateTimeKind.Utc).AddTicks(3370));
        }
    }
}
