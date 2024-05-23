using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBookingAppln.Migrations
{
    public partial class DriverDetailsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Drivers_UserDetails_UserDetailsUserId",
                table: "Drivers");

            migrationBuilder.DropIndex(
                name: "IX_Drivers_UserDetailsUserId",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "UserDetailsUserId",
                table: "Drivers");

            migrationBuilder.CreateTable(
                name: "DriverDetail",
                columns: table => new
                {
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    PasswordEncrypted = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordHashKey = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverDetail", x => x.DriverId);
                    table.ForeignKey(
                        name: "FK_DriverDetail_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverDetail");

            migrationBuilder.AddColumn<int>(
                name: "UserDetailsUserId",
                table: "Drivers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_UserDetailsUserId",
                table: "Drivers",
                column: "UserDetailsUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Drivers_UserDetails_UserDetailsUserId",
                table: "Drivers",
                column: "UserDetailsUserId",
                principalTable: "UserDetails",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
