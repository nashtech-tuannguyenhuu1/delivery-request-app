using Core.Data;
using Core.Domain;
using DeliveryRequest.Application.Entities;
using Mediator.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryRequest.Application.UseCases.Queries;
public class GetRequests
{
    public class Query : IQuery<IEnumerable<Request>>
    {

    }

    internal class Handler(IUnitOfWork unitOfWork) : IRequestHandler<Query, ResultModel<IEnumerable<Request>>>
    {
        public async Task<ResultModel<IEnumerable<Request>>> HandleAsync(Query request, CancellationToken cancellationToken)
        {
            var result = await unitOfWork.Repository<Request>().FindAsync();

            return new ResultModel<IEnumerable<Request>>(result);
        }
    }
}
