using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{
    public class Query : IRequest<List<Activity>> { }

    public class Handler(AppDbContext context) : IRequestHandler<Query, List<Activity>>
    {
        public Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
        {
            return context.Activities.ToListAsync(cancellationToken);
        }
    }
}