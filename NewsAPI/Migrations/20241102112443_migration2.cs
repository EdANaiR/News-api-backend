using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NewsAPI.Migrations
{
    /// <inheritdoc />
    public partial class migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CarouselNews",
                table: "CarouselNews");

            migrationBuilder.RenameColumn(
                name: "PublishDate",
                table: "CarouselNews",
                newName: "PublishedDate");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "CarouselNews",
                newName: "ShortDescription");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CarouselNews",
                newName: "CategoryId");

            migrationBuilder.AddColumn<Guid>(
                name: "CarouselId",
                table: "NewsImages",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CarouselId",
                table: "CarouselNews",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "CarouselNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Keywords",
                table: "CarouselNews",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarouselNews",
                table: "CarouselNews",
                column: "CarouselId");

            migrationBuilder.CreateIndex(
                name: "IX_NewsImages_CarouselId",
                table: "NewsImages",
                column: "CarouselId");

            migrationBuilder.CreateIndex(
                name: "IX_CarouselNews_CategoryId",
                table: "CarouselNews",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarouselNews_Categories_CategoryId",
                table: "CarouselNews",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsImages_CarouselNews_CarouselId",
                table: "NewsImages",
                column: "CarouselId",
                principalTable: "CarouselNews",
                principalColumn: "CarouselId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarouselNews_Categories_CategoryId",
                table: "CarouselNews");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsImages_CarouselNews_CarouselId",
                table: "NewsImages");

            migrationBuilder.DropIndex(
                name: "IX_NewsImages_CarouselId",
                table: "NewsImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CarouselNews",
                table: "CarouselNews");

            migrationBuilder.DropIndex(
                name: "IX_CarouselNews_CategoryId",
                table: "CarouselNews");

            migrationBuilder.DropColumn(
                name: "CarouselId",
                table: "NewsImages");

            migrationBuilder.DropColumn(
                name: "CarouselId",
                table: "CarouselNews");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "CarouselNews");

            migrationBuilder.DropColumn(
                name: "Keywords",
                table: "CarouselNews");

            migrationBuilder.RenameColumn(
                name: "ShortDescription",
                table: "CarouselNews",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "PublishedDate",
                table: "CarouselNews",
                newName: "PublishDate");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "CarouselNews",
                newName: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CarouselNews",
                table: "CarouselNews",
                column: "Id");
        }
    }
}
