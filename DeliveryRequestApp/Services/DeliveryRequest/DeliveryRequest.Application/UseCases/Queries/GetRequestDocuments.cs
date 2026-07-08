using AppContracts.DeliveryRequests.V1.Responses;
using Core.Data;
using Core.Domain;
using DeliveryRequest.Application.Entities;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Queries;

public class GetRequestDocuments
{
    public record Query(Guid RequestId) : IQuery<IEnumerable<DocumentResponseDto>>;

    internal class Handler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        : IRequestHandler<Query, ResultModel<IEnumerable<DocumentResponseDto>>>
    {
        public async Task<ResultModel<IEnumerable<DocumentResponseDto>>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var requestExists = await unitOfWork.Repository<Request>()
                .AnyAsync(x => x.Id == request.RequestId && x.CreatedById == currentUser.UserId, cancellationToken);

            if (!requestExists)
            {
                return new ResultModel<IEnumerable<DocumentResponseDto>>(default, true, "Request not found.");
            }

            var result = await unitOfWork.Repository<Document>()
                .FindAsync(
                    predicate: x => x.RequestId == request.RequestId,
                    orderBy: q => q.OrderByDescending(x => x.CreatedDate),
                    selector: x => new DocumentResponseDto
                    {
                        Id = x.Id,
                        RequestId = x.RequestId,
                        FileName = x.FileName,
                        ContentType = x.ContentType,
                        SizeBytes = x.SizeBytes,
                        CreatedDate = x.CreatedDate,
                    });

            return new ResultModel<IEnumerable<DocumentResponseDto>>(result);
        }
    }
}
