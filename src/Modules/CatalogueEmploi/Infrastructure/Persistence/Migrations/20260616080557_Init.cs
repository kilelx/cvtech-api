using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CatalogueEmploi.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Annonces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titre = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DomaineMetier = table.Column<string>(type: "TEXT", nullable: false),
                    TypeContrat = table.Column<int>(type: "INTEGER", nullable: false),
                    EntrepriseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    EstActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    DatePublication = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Annonces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Candidatures",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CandidatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    AnnonceId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CurriculumVitaeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LettreMotivation = table.Column<string>(type: "TEXT", nullable: true),
                    DateDepot = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurriculumsVitae",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CandidatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titre = table.Column<string>(type: "TEXT", nullable: false),
                    Resume = table.Column<string>(type: "TEXT", nullable: false),
                    DerniereModification = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurriculumsVitae", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Annonces");

            migrationBuilder.DropTable(
                name: "Candidatures");

            migrationBuilder.DropTable(
                name: "CurriculumsVitae");
        }
    }
}
