using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaDB.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEstadoServicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Estado",
                table: "Servicios",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Servicios");
        }
    }
}
