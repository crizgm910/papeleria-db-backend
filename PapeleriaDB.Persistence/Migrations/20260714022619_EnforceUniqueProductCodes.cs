using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PapeleriaDB.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnforceUniqueProductCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Productos_CodigoBarras",
                table: "Productos",
                column: "CodigoBarras",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_CodigoInterno",
                table: "Productos",
                column: "CodigoInterno",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Productos_CodigoBarras",
                table: "Productos");

            migrationBuilder.DropIndex(
                name: "IX_Productos_CodigoInterno",
                table: "Productos");
        }
    }
}
