using Fate.EfContext.Maps;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fate.EfContext
{
    public static class ModelBuilderExtension
    {
        public static ModelBuilder ConfigurationsDataTables(this ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new UserTagMap());

            return modelBuilder;


        }
    }
}
