using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VetiWebApplication.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDateConsulta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DtConsulta",
                table: "TB_CONSULTA",
                type: "NVARCHAR2(10)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "TIMESTAMP(7)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DtConsulta",
                table: "TB_CONSULTA",
                type: "TIMESTAMP(7)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR2(10)");
        }
    }
}
