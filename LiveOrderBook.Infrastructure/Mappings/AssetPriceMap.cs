using LiveOrderBook.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiveOrderBook.Infrastructure.Mappings;

public class AssetPriceMap : IEntityTypeConfiguration<AssetPrice>
{
    public void Configure(EntityTypeBuilder<AssetPrice> builder)
    {
        builder.ToTable("AssetPrices"); // Nome da tabela no banco

        builder.HasKey(x => x.Id); // Define a chave primária

        builder.Property(x => x.Asset)
            .IsRequired()
            .HasMaxLength(10); // Configuração de tamanho máximo para a coluna "Asset"

        builder.Property(x => x.Price)
            .IsRequired()
            .HasPrecision(18, 8); // Configuração de precisão para a coluna "Price"

        builder.Property(x => x.Quantity)
            .IsRequired()
            .HasPrecision(18, 8); // Configuração de precisão para a coluna "Quantity"

        builder.Property(x => x.Timestamp)
            .IsRequired(); // Define que "Timestamp" é obrigatório
    }
}