using Core.Storage;
using DeliveryRequest.Application.UseCases.Commands;
using DeliveryRequest.Application.UseCases.Queries;
using Mediator.Abstractions;

namespace DeliveryRequest.Api.Routes.V1;

public static class DocumentEndpointsV1
{
    public static void MapDocumentsEndpointsV1(this WebApplication app)
    {
        var group = app.MapGroup("/v1/requests/{requestId}/documents");

        group.MapGet("/", async (Guid requestId, ISender sender) =>
        {
            return await sender.SendAsync(new GetRequestDocuments.Query(requestId));
        });

        group.MapPost("/", async (Guid requestId, IFormFile file, ISender sender) =>
        {
            await using var stream = file.OpenReadStream();
            return await sender.SendAsync(new UploadDocument.Command(requestId, file.FileName, file.ContentType, file.Length, stream));
        }).DisableAntiforgery();

        app.MapGet("/v1/documents/{documentId}/download", async (Guid documentId, ISender sender, IBlobStorageService blobStorageService, HttpContext httpContext) =>
        {
            var result = await sender.SendAsync(new GetDocumentForDownload.Query(documentId));
            if (result.IsError || result.Data is null)
            {
                return Results.NotFound(result);
            }

            var stream = await blobStorageService.DownloadAsync(UploadDocument.ContainerName, result.Data.BlobName, httpContext.RequestAborted);
            return Results.File(stream, result.Data.ContentType, result.Data.FileName);
        });
    }
}
