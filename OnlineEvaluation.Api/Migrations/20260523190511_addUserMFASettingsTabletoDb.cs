using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineEvaluation.Api.Migrations
{
    /// <inheritdoc />
    public partial class addUserMFASettingsTabletoDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserMFASettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MFAType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "None"),
                    IsMFAEnabled = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SecretKey = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    BackupCodes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QRCodePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LastUsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMFASettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMFASettings_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMFASettings_ApplicationUserId",
                table: "UserMFASettings",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMFASettings");
        }
    }
}
