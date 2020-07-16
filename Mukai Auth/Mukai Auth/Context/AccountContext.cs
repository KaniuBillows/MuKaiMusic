using DataAbstract.Account;
using Microsoft.EntityFrameworkCore;

namespace Mukai_Auth.DataContext
{
    public class AccountContext : DbContext
    {
        public AccountContext(DbContextOptions<AccountContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>()
                .ToTable("UserInfo").HasIndex(u => new
                {
                    u.PhoneNumber,
                    u.Email,
                }).IsUnique();
        }
    }
}
