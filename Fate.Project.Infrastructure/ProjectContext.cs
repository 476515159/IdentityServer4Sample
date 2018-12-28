using Fate.Project.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fate.Project.Infrastructure
{
    public class ProjectContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;
        public DbSet<Domain.AggregatesModel.Project> Projects { get; set; }
        public DbSet<Domain.AggregatesModel.ProjectContributor> ProjectContributors { get; set; }
        public DbSet<Domain.AggregatesModel.ProjectProperty> ProjectPropertys { get; set; }
        public DbSet<Domain.AggregatesModel.ProjectViewer> ProjectViewers { get; set; }

        public DbSet<Domain.AggregatesModel.PrjectVisibleRule> PrjectVisibleRules { get; set; }

        public ProjectContext(DbContextOptions<ProjectContext> options, IMediator mediator) : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

            System.Diagnostics.Debug.WriteLine("ProjectContext::ctor ->" + this.GetHashCode());
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigurationsDataTables();
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mediator.DispatchDomainEventsAsync(this);
            var result = await base.SaveChangesAsync();

            return true;
        }
    }
}
