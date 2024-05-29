using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusBookingAppln.Migrations
{
    public partial class AddedColumnForPaymnet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "AmountPaid",
                table: "Payments",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AmountPaid",
                table: "Payments");
        }
    }
}
