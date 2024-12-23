using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class onetomany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentBarberServices");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AppointmentId",
                table: "Transactions");

            migrationBuilder.AddColumn<int>(
                name: "BarberServiceId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AppointmentId",
                table: "Transactions",
                column: "AppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BarberServiceId",
                table: "Appointments",
                column: "BarberServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_BarberServices_BarberServiceId",
                table: "Appointments",
                column: "BarberServiceId",
                principalTable: "BarberServices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_BarberServices_BarberServiceId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AppointmentId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_BarberServiceId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "BarberServiceId",
                table: "Appointments");

            migrationBuilder.CreateTable(
                name: "AppointmentBarberServices",
                columns: table => new
                {
                    AppointmentsId = table.Column<int>(type: "int", nullable: false),
                    ServicesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentBarberServices", x => new { x.AppointmentsId, x.ServicesId });
                    table.ForeignKey(
                        name: "FK_AppointmentBarberServices_Appointments_AppointmentsId",
                        column: x => x.AppointmentsId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentBarberServices_BarberServices_ServicesId",
                        column: x => x.ServicesId,
                        principalTable: "BarberServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AppointmentId",
                table: "Transactions",
                column: "AppointmentId",
                unique: true,
                filter: "[AppointmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentBarberServices_ServicesId",
                table: "AppointmentBarberServices",
                column: "ServicesId");
        }
    }
}
