using Application.Activities.Commands;
using Application.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

public class CommentHub : Hub
{
    private readonly IMediator mediator;

    public CommentHub(IMediator mediator)
    {
        this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task SendComment(AddComment.Command command)
    {
        var comment = await mediator.Send(command);
        await Clients
            .Group(command.ActivityId)
            .SendAsync("SendComment", comment.Value);
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();

        if (httpContext is null)
            throw new HubException("Unable to get HttpContext");

        var activityId = httpContext.Request.Query["activityId"];

        if (string.IsNullOrEmpty(activityId))
            throw new HubException("No activity with this ID");

        await Groups.AddToGroupAsync(Context.ConnectionId, activityId!);

        var result = await mediator.Send(new GetComments.Query { ActivityId = activityId! });

        await Clients.Caller.SendAsync("LoadComments", result.Value);
    }
}
