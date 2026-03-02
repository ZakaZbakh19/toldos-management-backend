using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModularBackend.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance
{
    public class IdentityUsersDbContext : IdentityDbContext<Users>
    {
        public IdentityUsersDbContext(DbContextOptions<IdentityUsersDbContext> options) :
            base(options)
        {

        }

        public IdentityUsersDbContext()
        {

        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
