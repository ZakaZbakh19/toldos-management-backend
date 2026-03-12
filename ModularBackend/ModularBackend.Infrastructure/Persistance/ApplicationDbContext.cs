using Microsoft.EntityFrameworkCore;
using ModularBackend.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModularBackend.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ProductsConfig());
        }

    }
}
