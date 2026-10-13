
using LoanApp_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LoanApp_API.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }


        

    public DbSet<LoanPayment> LoanPayment { get; set; } = null!;


    public DbSet<SupportTicket> SupportTickets { get; set; } = null!;
    public DbSet<LoanDeals> LoanDeals { get; set; } = null!;
    public DbSet<SanctionLetters> SanctionLetters { get; set; } = null!;
    public DbSet<Disbursements> Disbursements { get; set; } = null!;
    public DbSet<DealReviews> DealReviews { get; set; } = null!;
    public DbSet<Customers> Customers { get; set; } = null!;

    public DbSet<KycDocument> KycDocuments { get; set; } = null!;
    public DbSet<LoanAccount> LoanAccounts { get; set; } = null!;
    public DbSet<EmiSchedule> EmiSchedules { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;

    public DbSet<ForeClosureRequest> ForeClosureRequests { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<CibilReport> CibilReports { get; set; } = null!;
    public DbSet<ScoreCard> ScoreCards { get; set; } = null!;
    public DbSet<EligibilityResult> EligibilityResults { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var relationship in modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(e => e.GetForeignKeys()))
        {
            relationship.DeleteBehavior = DeleteBehavior.NoAction;
        }

        modelBuilder.Entity<ForeClosureRequest>(entity =>
        {
            entity.HasKey(x => x.RequestId);

            entity.Property(x => x.ForeClosureAmount)
                .HasColumnType("decimal(18,2)");


            // One loan account can have many foreclosure requests

            entity.Property(x => x.PartialAmount)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(x => x.LoanAccount)
                .WithMany()
                .HasForeignKey(x => x.LoanAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LoanAccount>()
            .HasOne(x => x.LoanDeal)
            .WithMany(x => x.LoanAccounts)
            .HasForeignKey(x => x.DealId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LoanAccount>()
            .HasOne(x => x.Customer)
            .WithMany(x => x.LoanAccounts)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmiSchedule>()
            .HasOne(x => x.LoanAccount)
            .WithMany(x => x.EmiSchedules)
            .HasForeignKey(x => x.LoanAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(x => x.Customer)
            .WithMany(x => x.Notifications)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Disbursements>()
            .HasOne(x => x.deals)
            .WithOne(x => x.disbursement)
            .HasForeignKey<Disbursements>(x => x.DealId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SanctionLetters>()
            .HasOne(x => x.deals)
            .WithOne(x => x.sanctionLetters)
            .HasForeignKey<SanctionLetters>(x => x.DealId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DealReviews>()
            .HasOne(x => x.deals)
            .WithOne(x => x.dealReviews)
            .HasForeignKey<DealReviews>(x => x.DealId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DealReviews>()
            .HasOne(x => x.users)
            .WithMany(x => x.DealReviews)
            .HasForeignKey(x => x.OfficerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<LoanDeals>()
            .HasOne(x => x.customers)
            .WithMany(x => x.loanDeals)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<KycDocument>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<SupportTicket>(entity =>
        {
            entity.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.LoanAccount)
                .WithMany()
                .HasForeignKey(x => x.LoanAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CibilReport>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ScoreCard>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EligibilityResult>()
            .HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
