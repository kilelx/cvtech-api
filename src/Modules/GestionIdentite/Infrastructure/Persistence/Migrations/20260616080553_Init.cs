using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionIdentite.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "gestion_identite");

            migrationBuilder.CreateTable(
                name: "Administrateurs",
                schema: "gestion_identite",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    MotDePasseHache = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<int>(type: "INTEGER", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrateurs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfilsCandidats",
                schema: "gestion_identite",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Prenom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Nom = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    MotDePasseHache = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurriculumVitaeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DateInscription = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilsCandidats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfilsEntreprises",
                schema: "gestion_identite",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    RaisonSociale = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Siret = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    MotDePasseHache = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", nullable: false),
                    EstActif = table.Column<bool>(type: "INTEGER", nullable: false),
                    DateInscription = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilsEntreprises", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProfilsCandidats_Email",
                schema: "gestion_identite",
                table: "ProfilsCandidats",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfilsEntreprises_Email",
                schema: "gestion_identite",
                table: "ProfilsEntreprises",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfilsEntreprises_Siret",
                schema: "gestion_identite",
                table: "ProfilsEntreprises",
                column: "Siret",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrateurs",
                schema: "gestion_identite");

            migrationBuilder.DropTable(
                name: "ProfilsCandidats",
                schema: "gestion_identite");

            migrationBuilder.DropTable(
                name: "ProfilsEntreprises",
                schema: "gestion_identite");
        }
    }
}
