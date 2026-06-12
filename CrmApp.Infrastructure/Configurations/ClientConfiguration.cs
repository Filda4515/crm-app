using CrmApp.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmApp.Infrastructure.Configurations;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.HasIndex(c => c.BirthNumber).IsUnique();

        builder.HasData(
            new Client
            {
                Id = 1,
                FirstName = "Jan",
                LastName = "Běžný",
                Email = "jan.bezny@gmail.com",
                Phone = "+420 123 456 789",
                BirthNumber = "960101/1234"
            },
            new Client
            {
                Id = 2,
                FirstName = "Alena",
                LastName = "Prázdná",
                Email = "alena.prazdna@gmail.com",
                Phone = "+420 000 000 000",
                BirthNumber = "955515/5555"
            },
            new Client
            {
                Id = 3,
                FirstName = "Štěpán",
                LastName = "Bývalý",
                Email = "stepan.byvaly@gmail.com",
                Phone = "+420 111 222 333",
                BirthNumber = "800505/1111"
            }
        );
    }
}
