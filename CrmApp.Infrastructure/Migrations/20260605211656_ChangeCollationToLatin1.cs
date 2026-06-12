using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrmApp.Infrastructure.Migrations;

/// <inheritdoc />
public partial class ChangeCollationToLatin1 : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase(
            collation: "Latin1_General_CI_AI",
            oldCollation: "Czech_CI_AI");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase(
            collation: "Czech_CI_AI",
            oldCollation: "Latin1_General_CI_AI");
    }
}
