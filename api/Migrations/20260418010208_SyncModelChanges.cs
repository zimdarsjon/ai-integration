using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignMembers_Campaigns_CampaignId",
                table: "CampaignMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CampaignSessions_Campaigns_CampaignId",
                table: "CampaignSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Encounters_Campaigns_CampaignId",
                table: "Encounters");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Spells",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Races",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Monsters",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Classes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "AbilityScores",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CustomContentShares",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContentType = table.Column<int>(type: "int", nullable: false),
                    ContentId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: false),
                    SharedWithUserId = table.Column<int>(type: "int", nullable: true),
                    SharedWithCampaignId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomContentShares", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomContentShares_Campaigns_SharedWithCampaignId",
                        column: x => x.SharedWithCampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CustomContentShares_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomContentShares_Users_SharedWithUserId",
                        column: x => x.SharedWithUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomContentShares_ContentType_ContentId_SharedWithUserId_SharedWithCampaignId",
                table: "CustomContentShares",
                columns: new[] { "ContentType", "ContentId", "SharedWithUserId", "SharedWithCampaignId" },
                unique: true,
                filter: "[SharedWithUserId] IS NOT NULL AND [SharedWithCampaignId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CustomContentShares_CreatedByUserId",
                table: "CustomContentShares",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomContentShares_SharedWithCampaignId",
                table: "CustomContentShares",
                column: "SharedWithCampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomContentShares_SharedWithUserId",
                table: "CustomContentShares",
                column: "SharedWithUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignMembers_Campaigns_CampaignId",
                table: "CampaignMembers",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignSessions_Campaigns_CampaignId",
                table: "CampaignSessions",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Encounters_Campaigns_CampaignId",
                table: "Encounters",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CampaignMembers_Campaigns_CampaignId",
                table: "CampaignMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_CampaignSessions_Campaigns_CampaignId",
                table: "CampaignSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Encounters_Campaigns_CampaignId",
                table: "Encounters");

            migrationBuilder.DropTable(
                name: "CustomContentShares");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Spells");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Races");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Monsters");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "AbilityScores");

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignMembers_Campaigns_CampaignId",
                table: "CampaignMembers",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CampaignSessions_Campaigns_CampaignId",
                table: "CampaignSessions",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Encounters_Campaigns_CampaignId",
                table: "Encounters",
                column: "CampaignId",
                principalTable: "Campaigns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
