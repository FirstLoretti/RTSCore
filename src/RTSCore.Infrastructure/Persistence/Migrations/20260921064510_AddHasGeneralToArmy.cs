using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RTSCore.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddHasGeneralToArmy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Armies_ArmyId",
                table: "Units");

            migrationBuilder.AddColumn<bool>(
                name: "HasGeneral",
                table: "Armies",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Armies_ArmyId",
                table: "Units",
                column: "ArmyId",
                principalTable: "Armies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Units_Armies_ArmyId",
                table: "Units");

            migrationBuilder.DropColumn(
                name: "HasGeneral",
                table: "Armies");

            migrationBuilder.AddForeignKey(
                name: "FK_Units_Armies_ArmyId",
                table: "Units",
                column: "ArmyId",
                principalTable: "Armies",
                principalColumn: "Id");
        }
    }
}
