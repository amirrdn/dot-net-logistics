using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Shipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AWB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    S_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    S_Tlp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    S_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    S_PostCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    R_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    R_Tlp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    R_Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    R_PostCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Dimension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AWB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentStatuses", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shipments");

            migrationBuilder.DropTable(
                name: "ShipmentStatuses");
        }
    }
}
