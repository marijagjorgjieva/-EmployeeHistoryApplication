using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeHistoryApplication.Migrations
{
    /// <inheritdoc />
    public partial class ChangesInHobHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobHistory_Employee_EmployeeId",
                table: "JobHistory");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "JobHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobHistory_Employee_EmployeeId",
                table: "JobHistory",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobHistory_Employee_EmployeeId",
                table: "JobHistory");

            migrationBuilder.AlterColumn<int>(
                name: "EmployeeId",
                table: "JobHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_JobHistory_Employee_EmployeeId",
                table: "JobHistory",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "Id");
        }
    }
}
