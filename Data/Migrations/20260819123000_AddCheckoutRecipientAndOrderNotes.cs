using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC_nhaSach.Data.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20260819123000_AddCheckoutRecipientAndOrderNotes")]
public partial class AddCheckoutRecipientAndOrderNotes : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Note",
            table: "Orders",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "FullName",
            table: "AspNetUsers",
            type: "nvarchar(100)",
            maxLength: 100,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "ShippingAddress",
            table: "AspNetUsers",
            type: "nvarchar(300)",
            maxLength: 300,
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "Note", table: "Orders");
        migrationBuilder.DropColumn(name: "FullName", table: "AspNetUsers");
        migrationBuilder.DropColumn(name: "ShippingAddress", table: "AspNetUsers");
    }
}
