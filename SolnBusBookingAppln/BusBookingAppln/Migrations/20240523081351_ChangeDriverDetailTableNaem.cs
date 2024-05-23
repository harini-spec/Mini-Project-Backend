using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBookingAppln.Migrations
{
    public partial class ChangeDriverDetailTableNaem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriverDetail_Drivers_DriverId",
                table: "DriverDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriverDetail",
                table: "DriverDetail");

            migrationBuilder.RenameTable(
                name: "DriverDetail",
                newName: "DriversDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriversDetails",
                table: "DriversDetails",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriversDetails_Drivers_DriverId",
                table: "DriversDetails",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DriversDetails_Drivers_DriverId",
                table: "DriversDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DriversDetails",
                table: "DriversDetails");

            migrationBuilder.RenameTable(
                name: "DriversDetails",
                newName: "DriverDetail");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DriverDetail",
                table: "DriverDetail",
                column: "DriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_DriverDetail_Drivers_DriverId",
                table: "DriverDetail",
                column: "DriverId",
                principalTable: "Drivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
