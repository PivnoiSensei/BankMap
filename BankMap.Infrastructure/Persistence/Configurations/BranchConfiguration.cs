using BankMap.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankMap.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("BankBranches");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Type)
            .HasConversion<string>()
            .IsRequired();

        // =========================
        // Address (OwnsOne)
        // =========================

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();
            address.Property(a => a.FullAddress)
                .HasMaxLength(300)
                .IsRequired();

            address.Property(a => a.Latitude);
            address.Property(a => a.Longitude);

            address.ToTable("BranchAddresses");
        });

        // =========================
        // Phones
        // =========================

        builder.OwnsMany(x => x.Phones, phone =>
        {
            phone.ToTable("BranchPhones");
            phone.WithOwner().HasForeignKey("BranchId");
            phone.Property<int>("Id");
            phone.HasKey("Id");
            phone.Property(p => p.OperatorCode)
                .HasMaxLength(10);
            phone.Property(p => p.Number)
                .HasMaxLength(20);
            phone.Ignore(p => p.FullNumber);
        });

        // =========================
        // Work Schedules
        // =========================

        builder.OwnsMany(x => x.Schedules, schedule =>
        {
            schedule.ToTable("BranchSchedules");
            schedule.WithOwner().HasForeignKey("BranchId");
            schedule.Property<int>("Id");
            schedule.HasKey("Id");
            schedule.Property(s => s.WorkStation)
              .HasMaxLength(50);

            //Working Days
            schedule.OwnsMany(s => s.Days, day =>
            {
                day.ToTable("BranchWorkingDays");
                day.WithOwner().HasForeignKey("ScheduleId");
                day.Property<int>("Id");
                day.HasKey("Id");

                day.Property(d => d.Day).HasConversion<int>();

                day.Property(d => d.From);
                day.Property(d => d.To);

                //Breaks
                day.OwnsMany(d => d.Breaks, br =>
                {
                    br.ToTable("BranchBreaks");
                    br.WithOwner().HasForeignKey("WorkingDayId");
                    br.Property<int>("Id");
                    br.HasKey("Id");

                    br.Property(b => b.From);
                    br.Property(b => b.To);
                });
            });
        });
        // =========================
        // Cash Desks (каси)
        // =========================
        builder.OwnsMany(x => x.CashDesks, cash =>
        {
            cash.ToTable("BranchCashDesks");
            cash.WithOwner().HasForeignKey("BranchId");
            cash.Property<int>("Id");
            cash.HasKey("Id");

            cash.Property(c => c.ExternalId);
            cash.Property(c => c.Description)
                .HasMaxLength(200);

            cash.OwnsMany(c => c.WorkDays, wd =>
            {
                wd.ToTable("CashDeskWorkingDays");
                wd.WithOwner().HasForeignKey("CashDeskId");

                wd.Property<int>("Id");
                wd.HasKey("Id");
                wd.Property(x => x.Day)
                    .HasConversion<int>();
                wd.Property(x => x.From);
                wd.Property(x => x.To);
            });
        });
    }
}
