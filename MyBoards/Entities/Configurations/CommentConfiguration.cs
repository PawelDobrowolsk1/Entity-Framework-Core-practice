using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MyBoards.Entities.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> eb)
    {
        eb.Property(x => x.CreatedDate).HasDefaultValueSql("getutcdate()");
        eb.Property(x => x.UpdatedDate).ValueGeneratedOnUpdate();
        eb.HasOne(c => c.Author)
            .WithMany(a => a.Comments)
            .HasForeignKey(c => c.AuthorId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}
