using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class AddedDeviceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "DeviceData");

            migrationBuilder.AddColumn<double>(
                name: "DeviceData_MOUSE2B_Resistance",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DeviceData_MOUSE2B_Voltage",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DeviceData_MOUSECOMBO_Resistance",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "DeviceData_MOUSECOMBO_Voltage",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "DeviceDataBase",
                type: "character varying(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Humidity",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LeakLocation",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reflectograms",
                table: "DeviceDataBase",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Resistance",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Temperature",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Voltage",
                table: "DeviceDataBase",
                type: "double precision",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceData_MOUSE2B_Resistance",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "DeviceData_MOUSE2B_Voltage",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "DeviceData_MOUSECOMBO_Resistance",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "DeviceData_MOUSECOMBO_Voltage",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "DeviceType",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "Humidity",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "LeakLocation",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "Reflectograms",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "Resistance",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "Temperature",
                table: "DeviceDataBase");

            migrationBuilder.DropColumn(
                name: "Voltage",
                table: "DeviceDataBase");

            migrationBuilder.AddColumn<string>(
                name: "DeviceType",
                table: "DeviceData",
                type: "character varying(13)",
                maxLength: 13,
                nullable: false,
                defaultValue: "");
        }
    }
}
