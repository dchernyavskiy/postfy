using System.Data;
using BuildingBlocks.Abstractions.CQRS.Events.Internal;
using BuildingBlocks.Abstractions.Persistence;
using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Postfy.Services.Identity.Shared.Models;

namespace Postfy.Services.Identity.Shared.Data;

public class IdentityContext
    : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        Guid,
        IdentityUserClaim<Guid>,
        ApplicationUserRole,
        IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>,
        IdentityUserToken<Guid>
    >,
        IDbFacadeResolver,
        IDomainEventContext,
        ITxDbContextExecution
{
    public IdentityContext(DbContextOptions<IdentityContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);


        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(entity.GetTableName()?.Underscore());

            var postfyObjectIdentifier = StoreObjectIdentifier.Table(
                entity.GetTableName()?.Underscore()!,
                entity.GetSchema()
            );

            // Replace column names
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName(postfyObjectIdentifier)?.Underscore());
            }

            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName()?.Underscore());
            }

            foreach (var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName()?.Underscore());
            }
        }
    }

    public Task ExecuteTransactionalAsync(Func<Task> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(
                IsolationLevel.ReadCommitted,
                cancellationToken
            );
            try
            {
                await action();

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public Task<T> ExecuteTransactionalAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        var strategy = Database.CreateExecutionStrategy();
        return strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await Database.BeginTransactionAsync(
                IsolationLevel.ReadCommitted,
                cancellationToken
            );
            try
            {
                var result = await action();

                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public IReadOnlyList<IDomainEvent> GetAllUncommittedEvents()
    {
        return new List<IDomainEvent>();
    }

    public void MarkUncommittedDomainEventAsCommitted()
    {
        // Method intentionally left empty.
    }
}
