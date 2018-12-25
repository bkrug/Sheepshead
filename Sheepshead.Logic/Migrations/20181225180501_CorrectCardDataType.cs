using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Migrations
{
    public partial class CorrectCardDataType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Card",
                table: "TrickPlay",
                type: "nchar(2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(2)");

            migrationBuilder.AlterColumn<string>(
                name: "Cards",
                table: "Participant",
                type: "nchar(35)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(35)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartnerCard",
                table: "Hand",
                type: "nchar(2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "char(2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuriedCards",
                table: "Hand",
                type: "nchar(6)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(6)");

            migrationBuilder.AlterColumn<string>(
                name: "BlindCards",
                table: "Hand",
                type: "nchar(6)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Card",
                table: "TrickPlay",
                type: "char(2)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(2)");

            migrationBuilder.AlterColumn<string>(
                name: "Cards",
                table: "Participant",
                type: "char(35)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(35)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PartnerCard",
                table: "Hand",
                type: "char(2)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nchar(2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuriedCards",
                table: "Hand",
                type: "char(6)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(6)");

            migrationBuilder.AlterColumn<string>(
                name: "BlindCards",
                table: "Hand",
                type: "char(6)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nchar(6)");
        }
    }
}
