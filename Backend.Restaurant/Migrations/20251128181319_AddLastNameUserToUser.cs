using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Restaurant.Migrations
{
    /// <inheritdoc />
    public partial class AddLastNameUserToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Users",
                type: "character varying(9)",
                maxLength: 9,
                nullable: true);
        }
    }
}
