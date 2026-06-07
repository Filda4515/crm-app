using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmApp.Migrations;

/// <inheritdoc />
public partial class AddUniqueConstraints : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Contracts_RegistrationNumber",
            table: "Contracts",
            column: "RegistrationNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Clients_BirthNumber",
            table: "Clients",
            column: "BirthNumber",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Advisors_BirthNumber",
            table: "Advisors",
            column: "BirthNumber",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Contracts_RegistrationNumber",
            table: "Contracts");

        migrationBuilder.DropIndex(
            name: "IX_Clients_BirthNumber",
            table: "Clients");

        migrationBuilder.DropIndex(
            name: "IX_Advisors_BirthNumber",
            table: "Advisors");
    }
}
