using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModularBackend.Application.Identity;
using ModularBackend.Infrastructure.Models.Identity;
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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(x => x.RefreshTokenId);

                entity.Property(x => x.TokenHash).IsRequired();
                entity.HasIndex(x => x.TokenHash).IsUnique();

                entity.HasOne<Users>()
                    .WithMany()
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(x => x.ReplacedByToken)
                    .WithMany()
                    .HasForeignKey(x => x.ReplacedByTokenId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
