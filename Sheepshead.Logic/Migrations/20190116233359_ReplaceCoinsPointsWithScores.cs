using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Migrations
{
    public partial class ReplaceCoinsPointsWithScores : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coin");

            migrationBuilder.DropTable(
                name: "Point");

            migrationBuilder.CreateTable(
                name: "Score",
                columns: table => new
                {
                    HandId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false),
                    Coins = table.Column<int>(nullable: false),
                    Points = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Score", x => new { x.HandId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_Score_Hand",
                        column: x => x.HandId,
                        principalTable: "Hand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Score_Participant",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FK_Score_Hand",
                table: "Score",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Score_Participant",
                table: "Score",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Score");

            migrationBuilder.CreateTable(
                name: "Coin",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Count = table.Column<int>(nullable: false),
                    HandId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coin", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hand_Coin",
                        column: x => x.HandId,
                        principalTable: "Hand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Coin_Participant",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Point",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HandId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Point", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointHand",
                        column: x => x.HandId,
                        principalTable: "Hand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Point_Participant",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FK_Hand_Coin",
                table: "Coin",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Coin_Participant",
                table: "Coin",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Point_HandId",
                table: "Point",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_Point_ParticipantId",
                table: "Point",
                column: "ParticipantId");
        }
    }
}
