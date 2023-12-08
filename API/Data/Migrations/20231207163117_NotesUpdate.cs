using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Data.Migrations
{
    /// <inheritdoc />
    public partial class NotesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Users_appUserId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "appUserId",
                table: "Notes",
                newName: "AppUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notes_appUserId",
                table: "Notes",
                newName: "IX_Notes_AppUserId");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Notes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Users_AppUserId",
                table: "Notes",
                column: "AppUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notes_Users_AppUserId",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Notes");

            migrationBuilder.RenameColumn(
                name: "AppUserId",
                table: "Notes",
                newName: "appUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Notes_AppUserId",
                table: "Notes",
                newName: "IX_Notes_appUserId");

            migrationBuilder.AlterColumn<int>(
                name: "appUserId",
                table: "Notes",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "Text",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Notes_Users_appUserId",
                table: "Notes",
                column: "appUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
