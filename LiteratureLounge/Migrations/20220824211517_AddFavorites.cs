using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteratureLounge.Migrations
{
    public partial class AddFavorites : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isFavorite",
                table: "Books",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isFavorite",
                table: "Books");
        }
    }
}
