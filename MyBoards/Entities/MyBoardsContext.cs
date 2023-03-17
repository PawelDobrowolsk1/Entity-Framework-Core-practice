﻿using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities;

public class MyBoardsContext : DbContext
{
    public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options)
    {

    }

    public DbSet<WorkItem> WorkItems { get; set; }
    public DbSet<Issue> Issues { get; set; }
    public DbSet<Epic> Epics { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<WorkItemState> WorkItemsStates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkItemState>()
             .Property(s => s.Value)
             .IsRequired()
             .HasMaxLength(50);


        modelBuilder.Entity<Epic>()
            .Property(wi => wi.EndDate)
            .HasPrecision(3);

        modelBuilder.Entity<Task>()
            .Property(wi => wi.Activity)
            .HasMaxLength(200);

        modelBuilder.Entity<Task>()
            .Property(wi => wi.RemaningWork)
            .HasPrecision(14, 2);

        modelBuilder.Entity<Issue>()
            .Property(wi => wi.Efford)
            .HasColumnType("decimal(5,2)");

        modelBuilder.Entity<WorkItem>(eb =>
        {
            eb.HasOne(w => w.State).WithMany().HasForeignKey(w => w.StateId);

            eb.Property(wi => wi.Area).HasColumnType("varchar(200)");
            eb.Property(wi => wi.IterationPath).HasColumnName("Iteration_Path");
            eb.Property(wi => wi.Priority).HasDefaultValue(1);

            eb.HasMany(w => w.Comments).WithOne(c => c.WorkItem).HasForeignKey(c => c.WorkItemId);
            eb.HasOne(w => w.Author).WithMany(u => u.WorkItems).HasForeignKey(w => w.AuthorId);

            eb.HasMany(w => w.Tags)
            .WithMany(t => t.WorkItems)
            .UsingEntity<WorkItemTag>(
                w => w.HasOne(wit => wit.Tag)
                .WithMany()
                .HasForeignKey(wit => wit.TagId),

                 w => w.HasOne(wit => wit.WorkItem)
                .WithMany()
                .HasForeignKey(wit => wit.WorkItemId),

                 wit =>
                 {
                     wit.HasKey(x => new { x.TagId, x.WorkItemId });
                     wit.Property(x => x.PublicationDate).HasDefaultValueSql("getutcdate()");
                 }
                );
        });

        modelBuilder.Entity<Comment>(eb =>
        {
            eb.Property(x => x.CreatedDate).HasDefaultValueSql("getutcdate()");
            eb.Property(x => x.UpdatedDate).ValueGeneratedOnUpdate();
            eb.HasOne(c => c.Author)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<User>()
            .HasOne(u => u.Address)
            .WithOne(u => u.User)
            .HasForeignKey<Address>(a => a.UserId);
    }
}
