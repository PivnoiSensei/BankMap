using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankMap.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankBranches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTemporaryClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsRegular = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankBranches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchAddresses",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchAddresses", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_BranchAddresses_BankBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BankBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchCashDesks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchCashDesks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchCashDesks_BankBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BankBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchPhones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OperatorCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchPhones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchPhones_BankBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BankBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkStation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchSchedules_BankBranches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "BankBranches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CashDeskWorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CashDeskId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CashDeskWorkingDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CashDeskWorkingDays_BranchCashDesks_CashDeskId",
                        column: x => x.CashDeskId,
                        principalTable: "BranchCashDesks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchWorkingDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ScheduleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchWorkingDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchWorkingDays_BranchSchedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "BranchSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchBreaks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    From = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    To = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WorkingDayId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchBreaks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchBreaks_BranchWorkingDays_WorkingDayId",
                        column: x => x.WorkingDayId,
                        principalTable: "BranchWorkingDays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchBreaks_WorkingDayId",
                table: "BranchBreaks",
                column: "WorkingDayId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchCashDesks_BranchId",
                table: "BranchCashDesks",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchPhones_BranchId",
                table: "BranchPhones",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchSchedules_BranchId",
                table: "BranchSchedules",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchWorkingDays_ScheduleId",
                table: "BranchWorkingDays",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_CashDeskWorkingDays_CashDeskId",
                table: "CashDeskWorkingDays",
                column: "CashDeskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchAddresses");

            migrationBuilder.DropTable(
                name: "BranchBreaks");

            migrationBuilder.DropTable(
                name: "BranchPhones");

            migrationBuilder.DropTable(
                name: "CashDeskWorkingDays");

            migrationBuilder.DropTable(
                name: "BranchWorkingDays");

            migrationBuilder.DropTable(
                name: "BranchCashDesks");

            migrationBuilder.DropTable(
                name: "BranchSchedules");

            migrationBuilder.DropTable(
                name: "BankBranches");
        }
    }
}
