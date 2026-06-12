using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CrmApp.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ExpandUsers : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.UpdateData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 1,
            columns: ["Email", "Phone"],
            values: ["petr.spravce@gmail.com", "+420 987 654 321"]);

        migrationBuilder.InsertData(
            table: "Advisors",
            columns: ["Id", "Age", "BirthNumber", "Email", "FirstName", "LastName", "Phone"],
            values: new object[,]
            {
                { 2, 34, "920303/9999", "karel.ucastnik@gmail.com", "Karel", "Účastník", "+420 111 222 333" },
                { 3, 36, "900101/0000", "pavel.prazdny@gmail.com", "Pavel", "Prázdný", "+420 555 666 777" }
            });

        migrationBuilder.UpdateData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 1,
            columns: ["Email", "Phone"],
            values: ["jan.novak@gmail.com", "+420 123 456 789"]);

        migrationBuilder.InsertData(
            table: "Clients",
            columns: ["Id", "Age", "BirthNumber", "Email", "FirstName", "LastName", "Phone"],
            values: [2, 28, "955555/5555", "alena.prazdna@gmail.com", "Alena", "Prázdná", "+420 000 000 000"]);

        migrationBuilder.InsertData(
            table: "AdvisorContract",
            columns: ["ParticipantsId", "ParticipatedContractsId"],
            values: [2, 1]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AdvisorContract",
            keyColumns: ["ParticipantsId", "ParticipatedContractsId"],
            keyValues: [2, 1]);

        migrationBuilder.DeleteData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.UpdateData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 1,
            columns: ["Email", "Phone"],
            values: [null, null]);

        migrationBuilder.UpdateData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 1,
            columns: ["Email", "Phone"],
            values: [null, null]);
    }
}
