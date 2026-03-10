using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailedAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DetailedAddress",
                table: "BranchAddresses",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailedAddress",
                table: "BranchAddresses");
        }
    }
}
