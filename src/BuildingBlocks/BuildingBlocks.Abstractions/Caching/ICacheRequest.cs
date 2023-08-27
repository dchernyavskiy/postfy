using MediatR;

namespace BuildingBlocks.Abstractions.Caching;



public interface ICacheRequest<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    TimeSpan AbsoluteExpirationRelativeToNow { get; }

    // TimeSpan SlidingExpiration { get; }
    // DateTime? AbsoluteExpiration { get; }
    string Prefix { get; }
    string CacheKey(TRequest request);
}

public interface IStreamCacheRequest<in TRequest, TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    TimeSpan AbsoluteExpirationRelativeToNow { get; }

    // TimeSpan SlidingExpiration { get; }
    // DateTime? AbsoluteExpiration { get; }
    string Prefix { get; }
    string CacheKey(TRequest request);
}
