using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTSCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddArmiesAndExtendCitiesAndUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OwnerFaction",
                table: "Units",
                newName: "Faction");

            migrationBuilder.AddColumn<string>(
                name: "ArmyId",
                table: "Units",
                type: "TEXT",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "Coordinates_X",
                table: "Cities",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "Coordinates_Y",
                table: "Cities",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "Governor",
                table: "Cities",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Armies",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    Faction = table.Column<string>(type: "TEXT", nullable: false),
                    GeneralId = table.Column<string>(type: "TEXT", nullable: false),
                    MovementPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxMovementPoints = table.Column<int>(type: "INTEGER", nullable: false),
                    MaxSize = table.Column<int>(type: "INTEGER", nullable: false),
                    Coordinates_X = table.Column<float>(type: "REAL", nullable: false),
                    Coordinates_Y = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Armies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Units_ArmyId",
                table: "Units",
                column: "ArmyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Armies_ArmyId",
                table: "Units",
                column: "ArmyId",
                principalTable: "Armies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Armies_ArmyId",
                table: "Units");

            migrationBuilder.DropTable(
                name: "Armies");

            migrationBuilder.DropIndex(
                name: "IX_Units_ArmyId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "ArmyId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "Coordinates_X",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Coordinates_Y",
                table: "Cities");

            migrationBuilder.DropColumn(
                name: "Governor",
                table: "Cities");

            migrationBuilder.RenameColumn(
                name: "Faction",
                table: "Units",
                newName: "OwnerFaction");
        }
    }
}
