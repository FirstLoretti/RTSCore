using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTSCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTurnsToRecruitToUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Faction",
                table: "Units",
                newName: "OwnerFaction");

            migrationBuilder.AddColumn<int>(
                name: "TurnsToRecruit",
                table: "Units",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TurnsToRecruit",
                table: "Units");

            migrationBuilder.RenameColumn(
                name: "OwnerFaction",
                table: "Units",
                newName: "Faction");
        }
    }
}
