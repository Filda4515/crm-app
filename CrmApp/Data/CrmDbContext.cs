using CrmApp.Models;

using Microsoft.EntityFrameworkCore;

namespace CrmApp.Data;

public class CrmDbContext(DbContextOptions<CrmDbContext> options) : DbContext(options)
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Advisor> Advisors { get; set; }
    public DbSet<Contract> Contracts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("Latin1_General_CI_AI");

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Manager)
            .WithMany(a => a.ManagedContracts)
            .HasForeignKey(c => c.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Contract>()
            .HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Client>().HasData(
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

        modelBuilder.Entity<Advisor>().HasData(
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

        modelBuilder.Entity<Contract>().HasData(
            new Contract
            {
                Id = 1,
                RegistrationNumber = "2026/001",
                Institution = "ČSOB",
                SignedDate = new DateTime(2026, 6, 7),
                EffectiveDate = new DateTime(2026, 7, 7),
                EndDate = null,
                ClientId = 1,
                ManagerId = 1
            },
            new Contract
            {
                Id = 2,
                RegistrationNumber = "2024/099",
                Institution = "Komerční banka",
                SignedDate = new DateTime(2024, 1, 15),
                EffectiveDate = new DateTime(2024, 2, 1),
                EndDate = new DateTime(2025, 12, 31),
                ClientId = 3,
                ManagerId = 4
            },
            new Contract
            {
                Id = 3,
                RegistrationNumber = "2026/002",
                Institution = "Kooperativa",
                SignedDate = new DateTime(2026, 1, 10),
                EffectiveDate = new DateTime(2026, 1, 10),
                EndDate = new DateTime(2035, 1, 1),
                ClientId = 1,
                ManagerId = 1
            }
        );

        modelBuilder.Entity<Contract>()
            .HasMany(c => c.Participants)
            .WithMany(a => a.ParticipatedContracts)
            .UsingEntity(j => j.HasData(
                new { ParticipantsId = 1, ParticipatedContractsId = 1 },
                new { ParticipantsId = 2, ParticipatedContractsId = 1 },
                new { ParticipantsId = 4, ParticipatedContractsId = 2 },
                new { ParticipantsId = 1, ParticipatedContractsId = 3 }
            ));
    }
}
