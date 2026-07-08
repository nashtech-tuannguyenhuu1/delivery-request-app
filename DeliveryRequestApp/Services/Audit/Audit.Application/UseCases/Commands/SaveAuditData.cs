using Core.Data;
using Core.Domain;
using Mediator.Abstractions;
using AuditEntity = Audit.Application.Entities.Audit;

namespace Audit.Application.UseCases.Commands;

public class SaveAuditData
{
    public record Command(AuditEntity AuditRecord) : ICommand<int>;

    internal class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Command, ResultModel<int>>
    {
        public async Task<ResultModel<int>> HandleAsync(Command request, CancellationToken cancellationToken)
        {
            await unitOfWork.Repository<AuditEntity>().AddAsync(request.AuditRecord, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return new ResultModel<int>(request.AuditRecord.Id);
        }
    }
}
