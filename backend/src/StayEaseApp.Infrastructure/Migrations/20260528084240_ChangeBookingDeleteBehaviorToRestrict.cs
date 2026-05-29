using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayEaseApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeBookingDeleteBehaviorToRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Properties_PropertyID",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Properties_PropertyID",
                table: "Bookings",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Properties_PropertyID",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Properties_PropertyID",
                table: "Bookings",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
