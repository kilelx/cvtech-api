using CVTech.Modules.GestionIdentite.Domain.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CVTech.Modules.GestionIdentite.Infrastructure.Persistence.Configurations;

public class ProfilEntrepriseConfiguration : IEntityTypeConfiguration<ProfilEntreprise>
{
    public void Configure(EntityTypeBuilder<ProfilEntreprise> builder)
    {
        builder.ToTable("ProfilsEntreprises");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RaisonSociale).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Siret).HasMaxLength(14).IsRequired();
        builder.HasIndex(x => x.Siret).IsUnique();
        builder.Property(x => x.Email).HasMaxLength(256).IsRequired();
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.MotDePasseHache).IsRequired();
        builder.Property(x => x.Role).HasConversion<string>();
    }
}
