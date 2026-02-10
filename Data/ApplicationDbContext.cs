using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReadyStatusApp.Models;

namespace ReadyStatusApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StatusLog> StatusLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StatusLog>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId);
        }
    }
}
