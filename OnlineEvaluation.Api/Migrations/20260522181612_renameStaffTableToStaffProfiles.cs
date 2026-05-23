using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineEvaluation.Api.Migrations
{
    /// <inheritdoc />
    public partial class renameStaffTableToStaffProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staff_AspNetUsers_ApplicationUserId",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_CollegeDepartments_CollegeDepartmentId",
                table: "Staff");

            migrationBuilder.DropForeignKey(
                name: "FK_Staff_Staff_ReportsToProfileId",
                table: "Staff");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Staff",
                table: "Staff");

            migrationBuilder.RenameTable(
                name: "Staff",
                newName: "StaffProfiles");

            migrationBuilder.RenameIndex(
                name: "IX_Staff_StaffGuid",
                table: "StaffProfiles",
                newName: "IX_StaffProfiles_StaffGuid");

            migrationBuilder.RenameIndex(
                name: "IX_Staff_ReportsToProfileId",
                table: "StaffProfiles",
                newName: "IX_StaffProfiles_ReportsToProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_Staff_EmployeeId",
                table: "StaffProfiles",
                newName: "IX_StaffProfiles_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Staff_CollegeDepartmentId",
                table: "StaffProfiles",
                newName: "IX_StaffProfiles_CollegeDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Staff_CollegeDepartmentAliasCode",
                table: "StaffProfiles",
                newName: "IX_StaffProfiles_CollegeDepartmentAliasCode");

            migrationBuilder.RenameIndex(
                name: "IX_Staff_ApplicationUserId",
                table: "StaffProfiles",
                newName: "IX_StaffProfiles_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StaffProfiles",
                table: "StaffProfiles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProfiles_AspNetUsers_ApplicationUserId",
                table: "StaffProfiles",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProfiles_CollegeDepartments_CollegeDepartmentId",
                table: "StaffProfiles",
                column: "CollegeDepartmentId",
                principalTable: "CollegeDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffProfiles_StaffProfiles_ReportsToProfileId",
                table: "StaffProfiles",
                column: "ReportsToProfileId",
                principalTable: "StaffProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffProfiles_AspNetUsers_ApplicationUserId",
                table: "StaffProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffProfiles_CollegeDepartments_CollegeDepartmentId",
                table: "StaffProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffProfiles_StaffProfiles_ReportsToProfileId",
                table: "StaffProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StaffProfiles",
                table: "StaffProfiles");

            migrationBuilder.RenameTable(
                name: "StaffProfiles",
                newName: "Staff");

            migrationBuilder.RenameIndex(
                name: "IX_StaffProfiles_StaffGuid",
                table: "Staff",
                newName: "IX_Staff_StaffGuid");

            migrationBuilder.RenameIndex(
                name: "IX_StaffProfiles_ReportsToProfileId",
                table: "Staff",
                newName: "IX_Staff_ReportsToProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_StaffProfiles_EmployeeId",
                table: "Staff",
                newName: "IX_Staff_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_StaffProfiles_CollegeDepartmentId",
                table: "Staff",
                newName: "IX_Staff_CollegeDepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_StaffProfiles_CollegeDepartmentAliasCode",
                table: "Staff",
                newName: "IX_Staff_CollegeDepartmentAliasCode");

            migrationBuilder.RenameIndex(
                name: "IX_StaffProfiles_ApplicationUserId",
                table: "Staff",
                newName: "IX_Staff_ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Staff",
                table: "Staff",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_AspNetUsers_ApplicationUserId",
                table: "Staff",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_CollegeDepartments_CollegeDepartmentId",
                table: "Staff",
                column: "CollegeDepartmentId",
                principalTable: "CollegeDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Staff_Staff_ReportsToProfileId",
                table: "Staff",
                column: "ReportsToProfileId",
                principalTable: "Staff",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
