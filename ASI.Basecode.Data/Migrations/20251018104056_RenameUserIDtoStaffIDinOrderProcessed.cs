using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIDtoStaffIDinOrderProcessed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcessed_RestaurantStaff_UserID",
                table: "OrderProcessed");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "OrderProcessed",
                newName: "StaffID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProcessed_UserID",
                table: "OrderProcessed",
                newName: "IX_OrderProcessed_StaffID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcessed_RestaurantStaff_StaffID",
                table: "OrderProcessed",
                column: "StaffID",
                principalTable: "RestaurantStaff",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProcessed_RestaurantStaff_StaffID",
                table: "OrderProcessed");

            migrationBuilder.RenameColumn(
                name: "StaffID",
                table: "OrderProcessed",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProcessed_StaffID",
                table: "OrderProcessed",
                newName: "IX_OrderProcessed_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProcessed_RestaurantStaff_UserID",
                table: "OrderProcessed",
                column: "UserID",
                principalTable: "RestaurantStaff",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
