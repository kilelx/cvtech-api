using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AppelOffreFreelance.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppelsOffre",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Titre = table.Column<string>(type: "TEXT", nullable: false),
                    Contexte = table.Column<string>(type: "TEXT", nullable: false),
                    DomaineMetier = table.Column<string>(type: "TEXT", nullable: false),
                    BudgetMax = table.Column<decimal>(type: "TEXT", nullable: false),
                    Deadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EntrepriseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Statut = table.Column<int>(type: "INTEGER", nullable: false),
                    DatePublication = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppelsOffre", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    AppelOffreId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CandidatId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TauxJournalierMoyen = table.Column<decimal>(type: "TEXT", nullable: false),
                    DureeEstimeeJours = table.Column<int>(type: "INTEGER", nullable: false),
                    Methodologie = table.Column<string>(type: "TEXT", nullable: false),
                    DateSoumission = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propositions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppelsOffre");

            migrationBuilder.DropTable(
                name: "Propositions");
        }
    }
}
