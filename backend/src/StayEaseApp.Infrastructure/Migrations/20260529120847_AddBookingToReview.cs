using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StayEaseApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookingToReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Properties_PropertyID",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserID",
                table: "Reviews");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingID",
                table: "Reviews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_BookingID",
                table: "Reviews",
                column: "BookingID",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Bookings_BookingID",
                table: "Reviews",
                column: "BookingID",
                principalTable: "Bookings",
                principalColumn: "BookingID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Properties_PropertyID",
                table: "Reviews",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserID",
                table: "Reviews",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Bookings_BookingID",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Properties_PropertyID",
                table: "Reviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Users_UserID",
                table: "Reviews");

            migrationBuilder.DropIndex(
                name: "IX_Reviews_BookingID",
                table: "Reviews");

            migrationBuilder.DropColumn(
                name: "BookingID",
                table: "Reviews");

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Properties_PropertyID",
                table: "Reviews",
                column: "PropertyID",
                principalTable: "Properties",
                principalColumn: "PropertyID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Users_UserID",
                table: "Reviews",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "UserID");
        }
    }
}
