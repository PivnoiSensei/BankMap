using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMap.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBranchModelForNewJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Branches",
                newName: "FullAddress");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Branches",
                newName: "DepartmentType");

            migrationBuilder.AddColumn<string>(
                name: "BaseCity",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DataJson",
                table: "Branches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsTemporaryClosed",
                table: "Branches",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseCity",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "DataJson",
                table: "Branches");

            migrationBuilder.DropColumn(
                name: "IsTemporaryClosed",
                table: "Branches");

            migrationBuilder.RenameColumn(
                name: "FullAddress",
                table: "Branches",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "DepartmentType",
                table: "Branches",
                newName: "Address");
        }
    }
}
