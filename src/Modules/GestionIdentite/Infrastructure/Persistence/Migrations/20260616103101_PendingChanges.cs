using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionIdentite.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstActif",
                schema: "gestion_identite",
                table: "Administrateurs",
                type: "INTEGER",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstActif",
                schema: "gestion_identite",
                table: "Administrateurs");
        }
    }
}
