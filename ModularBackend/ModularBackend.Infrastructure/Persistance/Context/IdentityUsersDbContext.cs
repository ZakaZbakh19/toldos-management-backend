using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Identity;
using ModularBackend.Infrastructure.Models.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance.Context
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RefreshTokenConfiguration());
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
