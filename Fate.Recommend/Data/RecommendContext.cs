using Fate.Recommend.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fate.Recommend.Data
{
    public class RecommendContext:DbContext
    {
        public RecommendContext(DbContextOptions<RecommendContext> options):base(options)
        {

        }

        public DbSet<RecommendProject> RecommendProjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecommendProject>().HasKey(c => c.Id);
            modelBuilder.Entity<RecommendProject>().Property(c=>c.Avatar).HasMaxLength(100);
            modelBuilder.Entity<RecommendProject>().Property(c => c.Company).HasMaxLength(100);
            modelBuilder.Entity<RecommendProject>().Property(c => c.UserName).HasMaxLength(100);
            modelBuilder.Entity<RecommendProject>().Property(c => c.FromUserName).HasMaxLength(100);
        }
    }
}
