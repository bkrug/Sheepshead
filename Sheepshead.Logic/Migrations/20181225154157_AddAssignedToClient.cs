using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Sheepshead.Logic.Migrations
{
    public partial class AddAssignedToClient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LeastersEnabled = table.Column<bool>(nullable: false),
                    PartnerMethod = table.Column<string>(type: "char(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Participant",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssignedToClient = table.Column<bool>(nullable: false),
                    Cards = table.Column<string>(type: "char(35)", nullable: true),
                    GameId = table.Column<Guid>(nullable: false),
                    Guid = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(unicode: false, nullable: true),
                    Type = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Player_Game",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hand",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BlindCards = table.Column<string>(type: "char(6)", nullable: false),
                    BuriedCards = table.Column<string>(type: "char(6)", nullable: false),
                    GameId = table.Column<Guid>(nullable: false),
                    PartnerCard = table.Column<string>(type: "char(2)", nullable: true),
                    PartnerParticipantId = table.Column<int>(nullable: true),
                    PickerParticipantId = table.Column<int>(nullable: true),
                    StartingParticipantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hand", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hand_Game",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Hand_Partner",
                        column: x => x.PartnerParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hand_Picker",
                        column: x => x.PickerParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hand_StartingParticipant",
                        column: x => x.StartingParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "ParticipantRefusingPick",
                columns: table => new
                {
                    HandId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParticipantRefusingPick", x => new { x.HandId, x.ParticipantId });
                    table.ForeignKey(
                        name: "FK_ParticipantRefusingPick_Hand",
                        column: x => x.HandId,
                        principalTable: "Hand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ParticipantRefusingPick_Participant",
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

            migrationBuilder.CreateTable(
                name: "Trick",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    HandId = table.Column<int>(nullable: false),
                    ParticipantId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trick", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trick_Hand",
                        column: x => x.HandId,
                        principalTable: "Hand",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trick_Participant",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TrickPlay",
                columns: table => new
                {
                    ParticipantId = table.Column<int>(nullable: false),
                    TrickId = table.Column<int>(nullable: false),
                    Card = table.Column<string>(type: "char(2)", nullable: false),
                    SortOrder = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrickPlay", x => new { x.ParticipantId, x.TrickId });
                    table.ForeignKey(
                        name: "FK_TrickPlay_Participant",
                        column: x => x.ParticipantId,
                        principalTable: "Participant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrickPlay_Trick",
                        column: x => x.TrickId,
                        principalTable: "Trick",
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
                name: "IX_FK_Hand_Game",
                table: "Hand",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Hand_Partner",
                table: "Hand",
                column: "PartnerParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Hand_Picker",
                table: "Hand",
                column: "PickerParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Hand_StartingParticipant",
                table: "Hand",
                column: "StartingParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Player_Game",
                table: "Participant",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_ParticipantRefusingPick_ParticipantId",
                table: "ParticipantRefusingPick",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_Point_HandId",
                table: "Point",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_Point_ParticipantId",
                table: "Point",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Trick_Hand",
                table: "Trick",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_Trick_Participant",
                table: "Trick",
                column: "ParticipantId");

            migrationBuilder.CreateIndex(
                name: "IX_FK_TrickPlay_Trick",
                table: "TrickPlay",
                column: "TrickId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coin");

            migrationBuilder.DropTable(
                name: "ParticipantRefusingPick");

            migrationBuilder.DropTable(
                name: "Point");

            migrationBuilder.DropTable(
                name: "TrickPlay");

            migrationBuilder.DropTable(
                name: "Trick");

            migrationBuilder.DropTable(
                name: "Hand");

            migrationBuilder.DropTable(
                name: "Participant");

            migrationBuilder.DropTable(
                name: "Game");
        }
    }
}
