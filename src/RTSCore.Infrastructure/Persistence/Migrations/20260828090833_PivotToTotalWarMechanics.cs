using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTSCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PivotToTotalWarMechanics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Buildings",
                newName: "TurnsToConstruct");

            migrationBuilder.RenameColumn(
                name: "Health",
                table: "Buildings",
                newName: "IsConstructed");

            migrationBuilder.RenameColumn(
                name: "Faction",
                table: "Buildings",
                newName: "OwnerFaction");

            migrationBuilder.AddColumn<string>(
                name: "CityId",
                table: "Buildings",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    OwnerFaction = table.Column<string>(type: "TEXT", nullable: false),
                    Population = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Factions",
                columns: table => new
                {
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    PlayerType = table.Column<int>(type: "INTEGER", nullable: false),
                    Gold = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factions", x => x.Type);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buildings_CityId",
                table: "Buildings",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Buildings_Cities_CityId",
                table: "Buildings",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Buildings_Cities_CityId",
                table: "Buildings");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Factions");

            migrationBuilder.DropIndex(
                name: "IX_Buildings_CityId",
                table: "Buildings");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "Buildings");

            migrationBuilder.RenameColumn(
                name: "TurnsToConstruct",
                table: "Buildings",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "OwnerFaction",
                table: "Buildings",
                newName: "Faction");

            migrationBuilder.RenameColumn(
                name: "IsConstructed",
                table: "Buildings",
                newName: "Health");
        }
    }
}
