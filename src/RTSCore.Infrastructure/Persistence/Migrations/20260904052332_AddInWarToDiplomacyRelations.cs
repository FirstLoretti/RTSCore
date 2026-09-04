using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTSCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddInWarToDiplomacyRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEliminated",
                table: "Factions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "DiplomacyOffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Initiator = table.Column<string>(type: "TEXT", nullable: false),
                    Target = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiplomacyOffers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiplomacyRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FactionA = table.Column<string>(type: "TEXT", nullable: false),
                    FactionB = table.Column<string>(type: "TEXT", nullable: false),
                    Standing = table.Column<int>(type: "INTEGER", nullable: false),
                    HasTradeAgreement = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsWar = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiplomacyRelations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiplomacyRelations_FactionA_FactionB",
                table: "DiplomacyRelations",
                columns: new[] { "FactionA", "FactionB" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiplomacyOffers");

            migrationBuilder.DropTable(
                name: "DiplomacyRelations");

            migrationBuilder.DropColumn(
                name: "IsEliminated",
                table: "Factions");
        }
    }
}
