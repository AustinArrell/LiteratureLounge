﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LiteratureLounge.Migrations
{
    public partial class addCoverLink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CoverLink",
                table: "Books",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoverLink",
                table: "Books");
        }
    }
}
