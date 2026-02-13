using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMap.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNewIsRegular : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRegular",
                table: "Branches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRegular",
                table: "Branches");
        }
    }
}
