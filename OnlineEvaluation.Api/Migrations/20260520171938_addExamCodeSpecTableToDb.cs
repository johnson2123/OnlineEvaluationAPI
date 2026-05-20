using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineEvaluation.Api.Migrations
{
    /// <inheritdoc />
    public partial class addExamCodeSpecTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExamCodeSpecifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Guid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExamSpecCode = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AcademicMapId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    InternalMaxMarks = table.Column<int>(type: "int", nullable: false),
                    ExternalMaxMarks = table.Column<int>(type: "int", nullable: false),
                    TotalMaxMarks = table.Column<int>(type: "int", nullable: false),
                    ExternalPassingMarks = table.Column<int>(type: "int", nullable: false),
                    TotalPassingMarks = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExamCodeSpecifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExamCodeSpecifications_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExamCodeSpecifications_AcademicMapId_Semester",
                table: "ExamCodeSpecifications",
                columns: new[] { "AcademicMapId", "Semester" });

            migrationBuilder.CreateIndex(
                name: "IX_ExamCodeSpecifications_ExamSpecCode",
                table: "ExamCodeSpecifications",
                column: "ExamSpecCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamCodeSpecifications_Guid",
                table: "ExamCodeSpecifications",
                column: "Guid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamCodeSpecifications_SubjectId",
                table: "ExamCodeSpecifications",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExamCodeSpecifications");
        }
    }
}
