using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmApp.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Advisors",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                BirthNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Age = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Advisors", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Clients",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                BirthNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Age = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Clients", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Contracts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                RegistrationNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                Institution = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                ClientId = table.Column<int>(type: "int", nullable: false),
                ManagerId = table.Column<int>(type: "int", nullable: false),
                SignedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Contracts", x => x.Id);
                table.ForeignKey(
                    name: "FK_Contracts_Advisors_ManagerId",
                    column: x => x.ManagerId,
                    principalTable: "Advisors",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
                table.ForeignKey(
                    name: "FK_Contracts_Clients_ClientId",
                    column: x => x.ClientId,
                    principalTable: "Clients",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "AdvisorContract",
            columns: table => new
            {
                ParticipantsId = table.Column<int>(type: "int", nullable: false),
                ParticipatedContractsId = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AdvisorContract", x => new { x.ParticipantsId, x.ParticipatedContractsId });
                table.ForeignKey(
                    name: "FK_AdvisorContract_Advisors_ParticipantsId",
                    column: x => x.ParticipantsId,
                    principalTable: "Advisors",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_AdvisorContract_Contracts_ParticipatedContractsId",
                    column: x => x.ParticipatedContractsId,
                    principalTable: "Contracts",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.InsertData(
            table: "Advisors",
            columns: ["Id", "Age", "BirthNumber", "Email", "FirstName", "LastName", "Phone"],
            values: [1, 41, "850202/5678", null, "Petr", "Poradce", null]);

        migrationBuilder.InsertData(
            table: "Clients",
            columns: ["Id", "Age", "BirthNumber", "Email", "FirstName", "LastName", "Phone"],
            values: [1, 30, "960101/1234", null, "Jan", "Novák", null]);

        migrationBuilder.InsertData(
            table: "Contracts",
            columns: ["Id", "ClientId", "EffectiveDate", "EndDate", "Institution", "ManagerId", "RegistrationNumber", "SignedDate"],
            values: [1, 1, new DateTime(2026, 7, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ČSOB", 1, "2026/001", new DateTime(2026, 6, 7, 0, 0, 0, 0, DateTimeKind.Unspecified)]);

        migrationBuilder.InsertData(
            table: "AdvisorContract",
            columns: ["ParticipantsId", "ParticipatedContractsId"],
            values: [1, 1]);

        migrationBuilder.CreateIndex(
            name: "IX_AdvisorContract_ParticipatedContractsId",
            table: "AdvisorContract",
            column: "ParticipatedContractsId");

        migrationBuilder.CreateIndex(
            name: "IX_Contracts_ClientId",
            table: "Contracts",
            column: "ClientId");

        migrationBuilder.CreateIndex(
            name: "IX_Contracts_ManagerId",
            table: "Contracts",
            column: "ManagerId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "AdvisorContract");

        migrationBuilder.DropTable(
            name: "Contracts");

        migrationBuilder.DropTable(
            name: "Advisors");

        migrationBuilder.DropTable(
            name: "Clients");
    }
}
