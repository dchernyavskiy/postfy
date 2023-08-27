using System.Linq.Expressions;
using BuildingBlocks.Abstractions.CQRS.Commands;
using BuildingBlocks.Abstractions.CQRS.Events.Internal;

namespace BuildingBlocks.Abstractions.Messaging.PersistMessage;






public interface IMessagePersistenceService
{
    Task<IReadOnlyList<StoreMessage>> GetByFilterAsync(
        Expression<Func<StoreMessage, bool>>? predicate = null,
        CancellationToken cancellationToken = default
    );

    Task AddPublishMessageAsync<TMessageEnvelope>(
        TMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken = default
    )
        where TMessageEnvelope : MessageEnvelope;

    Task AddReceivedMessageAsync<TMessageEnvelope>(
        TMessageEnvelope messageEnvelope,
        CancellationToken cancellationToken = default
    )
        where TMessageEnvelope : MessageEnvelope;

    Task AddInternalMessageAsync<TCommand>(TCommand internalCommand, CancellationToken cancellationToken = default)
        where TCommand : class, IInternalCommand;

    Task AddNotificationAsync(IDomainNotificationEvent notification, CancellationToken cancellationToken = default);

    Task ProcessAsync(Guid messageId, CancellationToken cancellationToken = default);

    Task ProcessAllAsync(CancellationToken cancellationToken = default);
}
