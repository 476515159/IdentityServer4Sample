using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Users;
using System;

namespace Fate.EfContext
{
    public class EntityDbContext:DbContext
    {
        public DbSet<UserInfo> UserInfo { get; set; }

        public DbSet<UserTags> UserTags { get; set; }


        public EntityDbContext(DbContextOptions<EntityDbContext> options):base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigurationsDataTables();
            base.OnModelCreating(modelBuilder);
        }



        public override void Dispose() => base.Dispose();
    }
}
