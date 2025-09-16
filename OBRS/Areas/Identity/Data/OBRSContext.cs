using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OBRS.Areas.Identity.Data;

namespace OBRS.Data
{
    public class OBRSContext : IdentityDbContext<OBRSUser>
    {
        public OBRSContext(DbContextOptions<OBRSContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply custom OBRSUser configuration
            builder.ApplyConfiguration(new OBRSUserConfiguration());
        }
    }

    internal class OBRSUserConfiguration : IEntityTypeConfiguration<OBRSUser>
    {
        public void Configure(EntityTypeBuilder<OBRSUser> builder)
        {
            builder.ToTable("AspNetUsers");

            builder.Property(u => u.FullName)
                   .HasColumnType("varchar(255)")
                   .IsRequired();

            builder.Property(u => u.PhoneNumber)
                   .HasColumnType("varchar(20)")
                   .IsRequired();

            builder.Property(u => u.CreatedAt)
                   .HasColumnType("datetime")
                   .IsRequired();

            builder.Property(u => u.IsActive)
                   .HasColumnType("bit")
                   .HasDefaultValue(true);
        }
    }
}
