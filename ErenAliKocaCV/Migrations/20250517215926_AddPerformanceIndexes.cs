using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErenAliKocaCV.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "ActivityLogs",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 5, 17, 21, 59, 25, 718, DateTimeKind.Utc).AddTicks(3370));

            migrationBuilder.CreateIndex(
                name: "IX_Skills_Category",
                table: "Skills",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Category",
                table: "Projects",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_DateSent",
                table: "ContactMessages",
                column: "DateSent");

            migrationBuilder.CreateIndex(
                name: "IX_ContactMessages_IsRead",
                table: "ContactMessages",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_Action",
                table: "ActivityLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_Action_IsSuccess_Timestamp",
                table: "ActivityLogs",
                columns: new[] { "Action", "IsSuccess", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_IsSuccess",
                table: "ActivityLogs",
                column: "IsSuccess");

            migrationBuilder.CreateIndex(
                name: "IX_ActivityLog_Timestamp",
                table: "ActivityLogs",
                column: "Timestamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Skills_Category",
                table: "Skills");

            migrationBuilder.DropIndex(
                name: "IX_Projects_Category",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_DateSent",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ContactMessages_IsRead",
                table: "ContactMessages");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLog_Action",
                table: "ActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLog_Action_IsSuccess_Timestamp",
                table: "ActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLog_IsSuccess",
                table: "ActivityLogs");

            migrationBuilder.DropIndex(
                name: "IX_ActivityLog_Timestamp",
                table: "ActivityLogs");

            migrationBuilder.AlterColumn<string>(
                name: "Action",
                table: "ActivityLogs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Degree = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FieldOfStudy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Institution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsCurrentlyStudying = table.Column<bool>(type: "bit", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AdminUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2025, 3, 24, 7, 53, 17, 396, DateTimeKind.Utc).AddTicks(5290));
        }
    }
}
