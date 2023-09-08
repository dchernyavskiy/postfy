using BuildingBlocks.Abstractions.CQRS.Events.Internal;
using BuildingBlocks.Core.Domain;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Core.Persistence.EfCore.Interceptors;

public class EventCommitterInterceptor : SaveChangesInterceptor
{
    private readonly IDomainEventPublisher _domainEventPublisher;

    public EventCommitterInterceptor(IDomainEventPublisher domainEventPublisher)
    {
        _domainEventPublisher = domainEventPublisher;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken()
    )
    {
        if (eventData.Context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        foreach (var entry in eventData.Context.ChangeTracker.Entries<Aggregate<Guid>>())
        {
            foreach (var @event in entry.Entity.DequeueUncommittedDomainEvents())
            {
                _domainEventPublisher.PublishAsync(@event, cancellationToken);
            }

            entry.Entity.MarkUncommittedDomainEventAsCommitted();
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
