using CrmApp.Domain.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CrmApp.Infrastructure.Configurations;

public class ContractConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        builder.HasIndex(c => c.RegistrationNumber).IsUnique();

        builder.HasOne(c => c.Manager)
            .WithMany(a => a.ManagedContracts)
            .HasForeignKey(c => c.ManagerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Client)
            .WithMany(cl => cl.Contracts)
            .HasForeignKey(c => c.ClientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(c => c.Participants)
            .WithMany(a => a.ParticipatedContracts)
            .UsingEntity(j => j.HasData(
                new { ParticipantsId = 1, ParticipatedContractsId = 1 },
                new { ParticipantsId = 2, ParticipatedContractsId = 1 },
                new { ParticipantsId = 4, ParticipatedContractsId = 2 },
                new { ParticipantsId = 1, ParticipatedContractsId = 3 }
            ));

        builder.HasData(
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
    }
}
