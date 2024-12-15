using Microsoft.EntityFrameworkCore;
using prometeyapi.Core.Models.Group;
using prometeyapi.Core.Models;
using Serilog;

namespace prometeyapi.Infrastructure.Data;

public class DBContext : DbContext
{
    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {
        Database.EnsureCreated();
        // Database.Migrate();
        Log.Debug("DBContext Initialized.");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection"), b => b.MigrationsAssembly("prometeyapi.WebApi"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Posts)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Applications)
            .WithOne(a => a.User)
            .HasForeignKey(a => a.UserId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.MemberOfGroups)
            .WithMany(g => g.Members);

        modelBuilder.Entity<User>()
            .HasMany(u => u.OwnedGroups)
            .WithOne(g => g.User)
            .HasForeignKey(g => g.UserId);
            
        modelBuilder.Entity<GroupSection>()
            .HasMany(g => g.Posts)
            .WithOne(p => p.Group)
            .HasForeignKey(p => p.GroupId);

        modelBuilder.Entity<GroupSection>()
            .HasMany(g => g.Applications)
            .WithOne(a => a.Group)
            .HasForeignKey(a => a.GroupId);

        modelBuilder.Entity<GroupPost>()
            .HasOne(p => p.User)
            .WithMany(u => u.GroupPosts)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<GroupApplication>()
            .HasOne(a => a.User)
            .WithMany(u => u.GroupApplications)
            .HasForeignKey(a => a.UserId);
    }

    // GENERAL
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Application> Applications { get; set; }

    // GROUPS
    public DbSet<GroupSection> Groups { get; set; }
    public DbSet<GroupPost> GroupPosts { get; set; }
    public DbSet<GroupApplication> GroupApplications { get; set; }
}
