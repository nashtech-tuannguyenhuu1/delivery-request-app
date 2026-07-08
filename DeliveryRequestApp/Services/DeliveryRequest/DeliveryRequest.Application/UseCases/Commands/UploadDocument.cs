using Core.Data;
using Core.Domain;
using Core.Storage;
using DeliveryRequest.Application.Entities;
using Mediator.Abstractions;

namespace DeliveryRequest.Application.UseCases.Commands;

public class UploadDocument
{
    public const string ContainerName = "documents";

    public record Command(Guid RequestId, string FileName, string ContentType, long SizeBytes, Stream Content) : ICommand<Guid>;

    internal class Handler(IUnitOfWork unitOfWork, ICurrentUser currentUser, IBlobStorageService blobStorageService)
        : IRequestHandler<Command, ResultModel<Guid>>
    {
        public async Task<ResultModel<Guid>> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            var requestExists = await unitOfWork.Repository<Request>()
                .AnyAsync(x => x.Id == request.RequestId && x.CreatedById == currentUser.UserId, cancellationToken);

            if (!requestExists)
            {
                return new ResultModel<Guid>(default, true, "Request not found.");
            }

            var blobName = $"{request.RequestId}/{Guid.NewGuid()}-{request.FileName}";
            await blobStorageService.UploadAsync(ContainerName, blobName, request.Content, request.ContentType, cancellationToken);

            var entity = new Document
            {
                Id = Guid.NewGuid(),
                RequestId = request.RequestId,
                FileName = request.FileName,
                ContentType = request.ContentType,
                SizeBytes = request.SizeBytes,
                BlobName = blobName,
            };

            await unitOfWork.Repository<Document>().AddAsync(entity, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResultModel<Guid>(entity.Id);
        }
    }
}
