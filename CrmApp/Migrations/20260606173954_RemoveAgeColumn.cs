using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmApp.Migrations;

/// <inheritdoc />
public partial class RemoveAgeColumn : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Age",
            table: "Clients");

        migrationBuilder.DropColumn(
            name: "Age",
            table: "Advisors");

        migrationBuilder.UpdateData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 2,
            column: "BirthNumber",
            value: "955515/5555");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Age",
            table: "Clients",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "Age",
            table: "Advisors",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.UpdateData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 1,
            column: "Age",
            value: 41);

        migrationBuilder.UpdateData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 2,
            column: "Age",
            value: 34);

        migrationBuilder.UpdateData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 3,
            column: "Age",
            value: 36);

        migrationBuilder.UpdateData(
            table: "Advisors",
            keyColumn: "Id",
            keyValue: 4,
            column: "Age",
            value: 28);

        migrationBuilder.UpdateData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 1,
            column: "Age",
            value: 30);

        migrationBuilder.UpdateData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 2,
            columns: ["Age", "BirthNumber"],
            values: [28, "955555/5555"]);

        migrationBuilder.UpdateData(
            table: "Clients",
            keyColumn: "Id",
            keyValue: 3,
            column: "Age",
            value: 46);
    }
}
