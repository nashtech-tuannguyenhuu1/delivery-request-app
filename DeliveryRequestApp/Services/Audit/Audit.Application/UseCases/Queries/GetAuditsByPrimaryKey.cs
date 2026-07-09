using Audit.Application.Responses;
using Core.Data;
using Core.Domain;
using Mediator.Abstractions;
using AuditEntity = Audit.Application.Entities.Audit;

namespace Audit.Application.UseCases.Queries;

public class GetAuditsByPrimaryKey
{
    public record Query(string PrimaryKey, string? TableName) : IQuery<List<AuditRecordDto>>;

    internal class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, ResultModel<List<AuditRecordDto>>>
    {
        public async Task<ResultModel<List<AuditRecordDto>>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.PrimaryKey))
            {
                return ResultModel<List<AuditRecordDto>>.Create(null, isError: true, errorMessage: "'PrimaryKey' is required.");
            }

            var records = await unitOfWork.Repository<AuditEntity>().FindAsync(
                selector: x => new AuditRecordDto
                {
                    Id = x.Id,
                    TableName = x.TableName,
                    Action = x.Action,
                    Timestamp = x.Timestamp,
                    UserId = x.UserId,
                    PrimaryKey = x.PrimaryKey,
                    Properties = x.AuditProperties.Select(p => new AuditPropertyDto
                    {
                        PropertyName = p.PropertyName,
                        OldValue = p.OldValue,
                        NewValue = p.NewValue,
                    }).ToList(),
                },
                predicate: x => x.PrimaryKey == request.PrimaryKey
                    && (request.TableName == null || x.TableName == request.TableName),
                orderBy: q => q.OrderByDescending(x => x.Timestamp),
                ct: cancellationToken);

            return ResultModel<List<AuditRecordDto>>.Create(records);
        }
    }
}
