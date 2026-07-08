using Core.Caching;
using Core.Domain;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Queries;

public class InvalidateRequestCache
{
    public record Query(Guid Id) : IQuery<bool>;

    public class Handler(ICacheManager cacheManager) : IRequestHandler<Query, ResultModel<bool>>
    {
        public async Task<ResultModel<bool>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.CacheKeys.GetRequestKey(request.Id);
            await cacheManager.RemoveAsync(cacheKey);

            return new ResultModel<bool>(true);
        }
    }
}
