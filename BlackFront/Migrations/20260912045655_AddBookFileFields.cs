using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlackFront.Migrations
{
    /// <inheritdoc />
    public partial class AddBookFileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadUrl",
                table: "Books",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "ReadUrl",
                table: "Books");
        }
    }
}
