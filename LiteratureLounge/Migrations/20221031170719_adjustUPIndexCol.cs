using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteratureLounge.Migrations
{
    public partial class adjustUPIndexCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferencesBookIndexColumn_IndexColumns_IndexColumnId",
                table: "UserPreferencesBookIndexColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferencesBookIndexColumn_UserPreferences_UserPreferenc~",
                table: "UserPreferencesBookIndexColumn");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferencesBookIndexColumn",
                table: "UserPreferencesBookIndexColumn");

            migrationBuilder.RenameTable(
                name: "UserPreferencesBookIndexColumn",
                newName: "UserPreferenceIndexColumns");

            migrationBuilder.RenameIndex(
                name: "IX_UserPreferencesBookIndexColumn_IndexColumnId",
                table: "UserPreferenceIndexColumns",
                newName: "IX_UserPreferenceIndexColumns_IndexColumnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferenceIndexColumns",
                table: "UserPreferenceIndexColumns",
                columns: new[] { "UserPreferencesId", "IndexColumnId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferenceIndexColumns_IndexColumns_IndexColumnId",
                table: "UserPreferenceIndexColumns",
                column: "IndexColumnId",
                principalTable: "IndexColumns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferenceIndexColumns_UserPreferences_UserPreferencesId",
                table: "UserPreferenceIndexColumns",
                column: "UserPreferencesId",
                principalTable: "UserPreferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferenceIndexColumns_IndexColumns_IndexColumnId",
                table: "UserPreferenceIndexColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_UserPreferenceIndexColumns_UserPreferences_UserPreferencesId",
                table: "UserPreferenceIndexColumns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserPreferenceIndexColumns",
                table: "UserPreferenceIndexColumns");

            migrationBuilder.RenameTable(
                name: "UserPreferenceIndexColumns",
                newName: "UserPreferencesBookIndexColumn");

            migrationBuilder.RenameIndex(
                name: "IX_UserPreferenceIndexColumns_IndexColumnId",
                table: "UserPreferencesBookIndexColumn",
                newName: "IX_UserPreferencesBookIndexColumn_IndexColumnId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserPreferencesBookIndexColumn",
                table: "UserPreferencesBookIndexColumn",
                columns: new[] { "UserPreferencesId", "IndexColumnId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferencesBookIndexColumn_IndexColumns_IndexColumnId",
                table: "UserPreferencesBookIndexColumn",
                column: "IndexColumnId",
                principalTable: "IndexColumns",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserPreferencesBookIndexColumn_UserPreferences_UserPreferenc~",
                table: "UserPreferencesBookIndexColumn",
                column: "UserPreferencesId",
                principalTable: "UserPreferences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
