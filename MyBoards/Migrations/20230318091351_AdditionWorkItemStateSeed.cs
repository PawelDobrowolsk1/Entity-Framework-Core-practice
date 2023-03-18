using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoards.Migrations
{
    /// <inheritdoc />
    public partial class AdditionWorkItemStateSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "WorkItemsStates",
                column: "Value",
                value: "On Hold");

            migrationBuilder.InsertData(
                table: "WorkItemsStates",
                column: "Value",
                value: "Rejected");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "WorkItemsStates",
                keyColumn: "Value",
                keyValue: "On Hold");

            migrationBuilder.DeleteData(
                table: "WorkItemsStates",
                keyColumn: "Value",
                keyValue: "Rejected");
        }
    }
}
