using Fate.Project.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.Project.Infrastructure
{
    public static class ModelBuilderExtension
    {
        public static ModelBuilder ConfigurationsDataTables(this ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectContributorConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectPropertyConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectViewerConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectVisibleRuleConfiguration());
            return modelBuilder;


        }
    }
}
