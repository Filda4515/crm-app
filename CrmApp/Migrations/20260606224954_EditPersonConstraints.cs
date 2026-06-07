using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmApp.Migrations;

/// <inheritdoc />
public partial class EditPersonConstraints : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "BirthNumber",
            table: "Clients",
            type: "nvarchar(11)",
            maxLength: 11,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "BirthNumber",
            table: "Advisors",
            type: "nvarchar(11)",
            maxLength: 11,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "BirthNumber",
            table: "Clients",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(11)",
            oldMaxLength: 11);

        migrationBuilder.AlterColumn<string>(
            name: "BirthNumber",
            table: "Advisors",
            type: "nvarchar(max)",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(11)",
            oldMaxLength: 11);
    }
}
