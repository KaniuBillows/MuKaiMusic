using DataAbstract.Account;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MuKai_Music.DataContext
{
    public class AccountContext : IdentityUserContext<UserInfo, int>
    {
        public AccountContext(DbContextOptions options) : base(options)
        {
        }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<UserInfo>()
                .ToTable("UserInfo").HasIndex(u => new
                {
                    u.PhoneNumber,
                    u.UserName,
                    u.Email,
                    u.Ne_Cellphone
                }).IsUnique();
        }
    }
}
