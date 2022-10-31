using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteratureLounge.Migrations
{
    public partial class adjustUP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "UserPreferences",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserPreferences",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserPreferencesBookIndexColumn",
                columns: table => new
                {
                    UserPreferencesId = table.Column<int>(type: "int", nullable: false),
                    IndexColumnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferencesBookIndexColumn", x => new { x.UserPreferencesId, x.IndexColumnId });
                    table.ForeignKey(
                        name: "FK_UserPreferencesBookIndexColumn_IndexColumns_IndexColumnId",
                        column: x => x.IndexColumnId,
                        principalTable: "IndexColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferencesBookIndexColumn_UserPreferences_UserPreferenc~",
                        column: x => x.UserPreferencesId,
                        principalTable: "UserPreferences",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferencesBookIndexColumn_IndexColumnId",
                table: "UserPreferencesBookIndexColumn",
                column: "IndexColumnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferencesBookIndexColumn");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserPreferences");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "UserPreferences",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "UserPreferencesId",
                table: "IndexColumns",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_IndexColumns_UserPreferencesId",
                table: "IndexColumns",
                column: "UserPreferencesId");

            migrationBuilder.AddForeignKey(
                name: "FK_IndexColumns_UserPreferences_UserPreferencesId",
                table: "IndexColumns",
                column: "UserPreferencesId",
                principalTable: "UserPreferences",
                principalColumn: "Id");
        }
    }
}
