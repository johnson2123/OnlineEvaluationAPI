using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineEvaluation.Api.Migrations
{
    /// <inheritdoc />
    public partial class addUniversitySubscriptionFieldstoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillingCycle",
                table: "Universities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PlanAmount",
                table: "Universities",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionEndDate",
                table: "Universities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionPlan",
                table: "Universities",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SubscriptionStartDate",
                table: "Universities",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubscriptionStatus",
                table: "Universities",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillingCycle",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "PlanAmount",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "SubscriptionEndDate",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "SubscriptionPlan",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "SubscriptionStartDate",
                table: "Universities");

            migrationBuilder.DropColumn(
                name: "SubscriptionStatus",
                table: "Universities");
        }
    }
}
