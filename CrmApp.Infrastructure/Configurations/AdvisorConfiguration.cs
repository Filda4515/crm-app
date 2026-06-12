using CrmApp.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmApp.Infrastructure.Configurations;

public class AdvisorConfiguration : IEntityTypeConfiguration<Advisor>
{
    public void Configure(EntityTypeBuilder<Advisor> builder)
    {
        builder.HasIndex(a => a.BirthNumber).IsUnique();

        builder.HasData(
            new Advisor
            {
                Id = 1,
                FirstName = "Petr",
                LastName = "Obojí",
                Email = "petr.oboji@gmail.com",
                Phone = "+420 987 654 321",
                BirthNumber = "850202/5678"
            },
            new Advisor
            {
                Id = 2,
                FirstName = "Karel",
                LastName = "Účastník",
                Email = "karel.ucastnik@gmail.com",
                Phone = "+420 111 222 333",
                BirthNumber = "920303/9999"
            },
            new Advisor
            {
                Id = 3,
                FirstName = "Pavel",
                LastName = "Prázdný",
                Email = "pavel.prazdny@gmail.com",
                Phone = "+420 555 666 777",
                BirthNumber = "900101/0000"
            },
            new Advisor
            {
                Id = 4,
                FirstName = "Žaneta",
                LastName = "Bývalá-Správcová",
                Email = "zaneta.spravcova@gmail.com",
                Phone = "+420 333 444 555",
                BirthNumber = "980808/8888"
            }
        );
    }
}
