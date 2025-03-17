using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShipmentId",
                table: "ShipmentStatuses",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentStatuses_ShipmentId",
                table: "ShipmentStatuses",
                column: "ShipmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentStatuses_Shipments_ShipmentId",
                table: "ShipmentStatuses",
                column: "ShipmentId",
                principalTable: "Shipments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentStatuses_Shipments_ShipmentId",
                table: "ShipmentStatuses");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentStatuses_ShipmentId",
                table: "ShipmentStatuses");

            migrationBuilder.DropColumn(
                name: "ShipmentId",
                table: "ShipmentStatuses");
        }
    }
}
