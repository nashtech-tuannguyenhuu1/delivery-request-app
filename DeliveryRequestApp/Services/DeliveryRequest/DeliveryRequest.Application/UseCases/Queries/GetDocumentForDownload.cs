using Core.Data;
using Core.Domain;
using DeliveryRequest.Application.Entities;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Queries;

public class GetDocumentForDownload
{
    public record DocumentDownloadInfo(string BlobName, string FileName, string ContentType);

    public record Query(Guid DocumentId) : IQuery<DocumentDownloadInfo>;

    internal class Handler(IUnitOfWork unitOfWork, ICurrentUser currentUser)
        : IRequestHandler<Query, ResultModel<DocumentDownloadInfo>>
    {
        public async Task<ResultModel<DocumentDownloadInfo>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Repository<Document>()
                .FirstOrDefaultAsync(
                    predicate: x => x.Id == request.DocumentId,
                    selector: x => new { x.BlobName, x.FileName, x.ContentType, x.RequestId },
                    ct: cancellationToken);

            if (result is null)
            {
                return new ResultModel<DocumentDownloadInfo>(default, true, "Document not found.");
            }

            var ownedByCurrentUser = await unitOfWork.Repository<Request>()
                .AnyAsync(x => x.Id == result.RequestId && x.CreatedById == currentUser.UserId, cancellationToken);

            if (!ownedByCurrentUser)
            {
                return new ResultModel<DocumentDownloadInfo>(default, true, "Document not found.");
            }

            return new ResultModel<DocumentDownloadInfo>(new DocumentDownloadInfo(result.BlobName, result.FileName, result.ContentType));
        }
    }
}
