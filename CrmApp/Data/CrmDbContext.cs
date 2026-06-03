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
            new Client { Id = 1, FirstName = "Jan", LastName = "Novák", BirthNumber = "960101/1234", Age = 30 }
        );

        modelBuilder.Entity<Advisor>().HasData(
            new Advisor { Id = 1, FirstName = "Petr", LastName = "Poradce", BirthNumber = "850202/5678", Age = 41 }
        );

        modelBuilder.Entity<Contract>().HasData(
            new Contract
            {
                Id = 1,
                RegistrationNumber = "2026/001",
                Institution = "ČSOB",
                SignedDate = new DateTime(2026, 6, 7),
                EffectiveDate = new DateTime(2026, 7, 7),
                ClientId = 1,
                ManagerId = 1
            }
        );

        modelBuilder.Entity<Contract>()
            .HasMany(c => c.Participants)
            .WithMany(a => a.ParticipatedContracts)
            .UsingEntity(j => j.HasData(
                new { ParticipantsId = 1, ParticipatedContractsId = 1 }
            ));
    }
}
