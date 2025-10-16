using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixUserAddressRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key first
            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_Addresses_AddressID",
                table: "UserAddresses");

            // Drop the unique index
            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_AddressID",
                table: "UserAddresses");

            // Create a new non-unique index
            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_AddressID",
                table: "UserAddresses",
                column: "AddressID");

            // Recreate the foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_Addresses_AddressID",
                table: "UserAddresses",
                column: "AddressID",
                principalTable: "Addresses",
                principalColumn: "AddressID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key first
            migrationBuilder.DropForeignKey(
                name: "FK_UserAddresses_Addresses_AddressID",
                table: "UserAddresses");

            // Drop the non-unique index
            migrationBuilder.DropIndex(
                name: "IX_UserAddresses_AddressID",
                table: "UserAddresses");

            // Create the unique index
            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_AddressID",
                table: "UserAddresses",
                column: "AddressID",
                unique: true);

            // Recreate the foreign key
            migrationBuilder.AddForeignKey(
                name: "FK_UserAddresses_Addresses_AddressID",
                table: "UserAddresses",
                column: "AddressID",
                principalTable: "Addresses",
                principalColumn: "AddressID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
