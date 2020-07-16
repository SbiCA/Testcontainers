using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SampleApp.Api
{
    public class ReviewHub : Hub<IReviewSubscriptions>
    {
        private readonly ILogger<ReviewHub> _logger;

        public ReviewHub(ILogger<ReviewHub> logger)
        {
            _logger = logger;
        }

        public async Task SubscribeMovie(string movieId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, movieId);
            _logger.LogInformation("Added connection {connectionId} to group {installationId}", Context.ConnectionId, movieId);
        }

        public async Task UnsubscribeFromMovie(string movieId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, movieId);
            _logger.LogInformation("Removed connection {connectionId} from group {installationId}", Context.ConnectionId, movieId);
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation("Api client disconnected {connectionId} reason {@exception}", Context.ConnectionId, exception);
            return base.OnDisconnectedAsync(exception);
        }
    }
}