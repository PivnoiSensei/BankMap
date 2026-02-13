using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMap.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddExternalDepartmentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalDepartmentId",
                table: "Branches",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalDepartmentId",
                table: "Branches");
        }
    }
}
