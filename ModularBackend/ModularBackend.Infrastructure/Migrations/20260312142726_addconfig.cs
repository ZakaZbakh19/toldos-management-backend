using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ModularBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addconfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BasePrice_Currency",
                table: "Products",
                newName: "BasePriceCurrency");

            migrationBuilder.RenameColumn(
                name: "BasePrice_Amount",
                table: "Products",
                newName: "BasePriceAmount");

            migrationBuilder.AlterColumn<string>(
                name: "BasePriceCurrency",
                table: "Products",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BasePriceCurrency",
                table: "Products",
                newName: "BasePrice_Currency");

            migrationBuilder.RenameColumn(
                name: "BasePriceAmount",
                table: "Products",
                newName: "BasePrice_Amount");

            migrationBuilder.AlterColumn<int>(
                name: "BasePrice_Currency",
                table: "Products",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(3)",
                oldMaxLength: 3);
        }
    }
}
