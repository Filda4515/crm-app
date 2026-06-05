using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmApp.Migrations;

/// <inheritdoc />
public partial class AddFilterTestSeeds : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase(
            collation: "Czech_CI_AI");

        migrationBuilder.UpdateData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 1,
            columns: ["Email", "LastName"],
            values: ["petr.oboji@gmail.com", "Obojí"]);

        migrationBuilder.InsertData(
            table: "Advisors",
            columns: ["Id", "Age", "BirthNumber", "Email", "FirstName", "LastName", "Phone"],
            values: [4, 28, "980808/8888", "zaneta.spravcova@gmail.com", "Žaneta", "Bývalá-Správcová", "+420 333 444 555"]);

        migrationBuilder.UpdateData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 1,
            columns: ["Email", "LastName"],
            values: ["jan.bezny@gmail.com", "Běžný"]);

        migrationBuilder.InsertData(
            table: "Clients",
            columns: ["Id", "Age", "BirthNumber", "Email", "FirstName", "LastName", "Phone"],
            values: [3, 46, "800505/1111", "stepan.byvaly@gmail.com", "Štěpán", "Bývalý", "+420 111 222 333"]);

        migrationBuilder.InsertData(
            table: "Contracts",
            columns: ["Id", "ClientId", "EffectiveDate", "EndDate", "Institution", "ManagerId", "RegistrationNumber", "SignedDate"],
            values: [3, 1, new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2035, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Kooperativa", 1, "2026/002", new DateTime(2026, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified)]);

        migrationBuilder.InsertData(
            table: "AdvisorContract",
            columns: ["ParticipantsId", "ParticipatedContractsId"],
            values: [1, 3]);

        migrationBuilder.InsertData(
            table: "Contracts",
            columns: ["Id", "ClientId", "EffectiveDate", "EndDate", "Institution", "ManagerId", "RegistrationNumber", "SignedDate"],
            values: [2, 3, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "Komerční banka", 4, "2024/099", new DateTime(2024, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified)]);

        migrationBuilder.InsertData(
            table: "AdvisorContract",
            columns: ["ParticipantsId", "ParticipatedContractsId"],
            values: [4, 2]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData(
            table: "AdvisorContract",
            keyColumns: ["ParticipantsId", "ParticipatedContractsId"],
            keyValues: [1, 3]);

        migrationBuilder.DeleteData(
            table: "AdvisorContract",
            keyColumns: ["ParticipantsId", "ParticipatedContractsId"],
            keyValues: [4, 2]);

        migrationBuilder.DeleteData(
            table: "Contracts",
            keyColumn: "Id",
            keyValue: 2);

        migrationBuilder.DeleteData(
            table: "Contracts",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.DeleteData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 4);

        migrationBuilder.DeleteData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 3);

        migrationBuilder.AlterDatabase(
            oldCollation: "Czech_CI_AI");

        migrationBuilder.UpdateData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 1,
            columns: ["Email", "LastName"],
            values: ["petr.spravce@gmail.com", "Poradce"]);

        migrationBuilder.UpdateData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 1,
            columns: ["Email", "LastName"],
            values: ["jan.novak@gmail.com", "Novák"]);
    }
}
