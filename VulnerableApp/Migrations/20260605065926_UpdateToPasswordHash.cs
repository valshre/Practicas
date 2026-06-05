using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VulnerableApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Users",
                newName: "PasswordHash");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "$2a$11$R5rvB/DCtoBE.iCWDsJ/G.fcufduEii8U0dGaF2ANxTpb.5OojKj2" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "$2a$11$T3kiWHcV4BS6MIxfXg7JE.y1MHu1/SKtnWxvDa82.eZwh42SiFA9e" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "$2a$11$EYboFMOJsScBuWxbOtzq2uwFCgEws.uDvBYcqdpS9n3PiCoi9hv7a" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Users",
                newName: "Password");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 6, 1, 19, 25, 39, 694, DateTimeKind.Local).AddTicks(3549), "admin" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 6, 1, 19, 25, 39, 699, DateTimeKind.Local).AddTicks(1446), "123456" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Password" },
                values: new object[] { new DateTime(2026, 6, 1, 19, 25, 39, 699, DateTimeKind.Local).AddTicks(1542), "password" });
        }
    }
}
