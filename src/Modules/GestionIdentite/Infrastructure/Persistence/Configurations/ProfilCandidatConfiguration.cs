using CVTech.Modules.GestionIdentite.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVTech.Modules.GestionIdentite.Infrastructure.Persistence.Configurations;

public class ProfilCandidatConfiguration : IEntityTypeConfiguration<ProfilCandidat>
{
    public void Configure(EntityTypeBuilder<ProfilCandidat> builder)
    {
        builder.ToTable("ProfilsCandidats");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Prenom).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Nom).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.MotDePasseHache).IsRequired();
        builder.Property(x => x.Role).HasConversion<string>();
    }
}
