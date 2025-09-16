using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBRS.Migrations
{
    /// <inheritdoc />
    public partial class InitAuth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tbl_contact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_contact", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_route",
                columns: table => new
                {
                    RouteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteName = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    StartLocation = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    Destination = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    DistanceInKm = table.Column<int>(type: "int", nullable: false),
                    EstimatedDuration = table.Column<TimeSpan>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_route", x => x.RouteId);
                });

            migrationBuilder.CreateTable(
                name: "tbl_bus",
                columns: table => new
                {
                    BusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BusName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BusType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TotalSeats = table.Column<int>(type: "int", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PricePerSeat = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    BusImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Route_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_bus", x => x.BusId);
                    table.ForeignKey(
                        name: "FK_tbl_bus_tbl_route_Route_id",
                        column: x => x.Route_id,
                        principalTable: "tbl_route",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_bookings",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PassengerName = table.Column<string>(type: "varchar(100)", nullable: false),
                    PassengerCNIC = table.Column<string>(type: "varchar(15)", nullable: false),
                    PassengerPhone = table.Column<string>(type: "varchar(15)", nullable: false),
                    TravelDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SeatNumber = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bus_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_tbl_bookings_tbl_bus_Bus_id",
                        column: x => x.Bus_id,
                        principalTable: "tbl_bus",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_employees",
                columns: table => new
                {
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullName = table.Column<string>(type: "varchar(100)", nullable: false),
                    CNIC = table.Column<string>(type: "varchar(15)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "varchar(15)", nullable: false),
                    Address = table.Column<string>(type: "varchar(200)", nullable: false),
                    Role = table.Column<string>(type: "varchar(20)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "varchar(50)", nullable: true),
                    HireDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EmployeeImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bus_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_employees", x => x.EmployeeId);
                    table.ForeignKey(
                        name: "FK_tbl_employees_tbl_bus_Bus_id",
                        column: x => x.Bus_id,
                        principalTable: "tbl_bus",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tbl_price",
                columns: table => new
                {
                    PriceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseFare = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    AdditionalCharges = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FinalFare = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Bus_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_price", x => x.PriceId);
                    table.ForeignKey(
                        name: "FK_tbl_price_tbl_bus_Bus_id",
                        column: x => x.Bus_id,
                        principalTable: "tbl_bus",
                        principalColumn: "BusId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bookings_Bus_id",
                table: "tbl_bookings",
                column: "Bus_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_bus_Route_id",
                table: "tbl_bus",
                column: "Route_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_employees_Bus_id",
                table: "tbl_employees",
                column: "Bus_id");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_price_Bus_id",
                table: "tbl_price",
                column: "Bus_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_bookings");

            migrationBuilder.DropTable(
                name: "tbl_contact");

            migrationBuilder.DropTable(
                name: "tbl_employees");

            migrationBuilder.DropTable(
                name: "tbl_price");

            migrationBuilder.DropTable(
                name: "tbl_bus");

            migrationBuilder.DropTable(
                name: "tbl_route");
        }
    }
}
