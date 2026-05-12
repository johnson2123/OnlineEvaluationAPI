using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineEvaluation.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUniversity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UniversityId",
                table: "Colleges",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colleges_UniversityId",
                table: "Colleges",
                column: "UniversityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_Universities_UniversityId",
                table: "Colleges",
                column: "UniversityId",
                principalTable: "Universities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_Universities_UniversityId",
                table: "Colleges");

            migrationBuilder.DropIndex(
                name: "IX_Colleges_UniversityId",
                table: "Colleges");

            migrationBuilder.DropColumn(
                name: "UniversityId",
                table: "Colleges");
        }
    }
}
