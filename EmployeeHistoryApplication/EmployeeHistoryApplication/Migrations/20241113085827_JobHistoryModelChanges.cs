using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeHistoryApplication.Migrations
{
    /// <inheritdoc />
    public partial class JobHistoryModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "JobHistory");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "JobHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobPostition",
                table: "JobHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "JobHistory");

            migrationBuilder.DropColumn(
                name: "JobPostition",
                table: "JobHistory");

            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "JobHistory",
                type: "int",
                nullable: true);
        }
    }
}
