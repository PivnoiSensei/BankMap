using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CashDeskbreaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CashDeskBreaks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CashDeskWorkingDayId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashDeskBreaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashDeskBreaks_CashDeskWorkingDays_CashDeskWorkingDayId",
                        column: x => x.CashDeskWorkingDayId,
                        principalTable: "CashDeskWorkingDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CashDeskBreaks_CashDeskWorkingDayId",
                table: "CashDeskBreaks",
                column: "CashDeskWorkingDayId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CashDeskBreaks");
        }
    }
}
