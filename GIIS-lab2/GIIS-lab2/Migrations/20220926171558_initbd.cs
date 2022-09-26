using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GIIS_lab2.Migrations
{
    public partial class initbd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[] { new Guid("214f5fc6-a34f-4e25-8d7e-9ffe1f0da122"), "Bombass", "Dranik" });

            migrationBuilder.InsertData(
                table: "Contacts",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[] { new Guid("327d90ac-9247-4f29-8231-c9650d499ba4"), "Kolotushkino", "Alex" });

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_Name_Address",
                table: "Contacts",
                columns: new[] { "Name", "Address" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contacts");
        }
    }
}
