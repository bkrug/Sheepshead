using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Migrations
{
    public partial class CorrectDataTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParticipantId",
                table: "Trick",
                newName: "StartingParticipantId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Participant",
                type: "char(1)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Participant",
                type: "nvarchar(max)",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldUnicode: false,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartingParticipantId",
                table: "Trick",
                newName: "ParticipantId");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Participant",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "char(1)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Participant",
                unicode: false,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldUnicode: false,
                oldNullable: true);
        }
    }
}
