using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SampleApp.Persistence;

namespace SampleApp.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : Controller
    {
        private readonly IHubContext<ReviewHub, IReviewSubscriptions> _hubContext;
        private readonly ReviewRepository _repository;

        public ReviewsController(ReviewRepository repository, IHubContext<ReviewHub, IReviewSubscriptions> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        [HttpPost("{movie}/ratings")]
        public async Task<IActionResult> AddRating([FromRoute] string movie, AddRating command)
        {
            await _repository.AddRating(movie, command.User, command.Stars);
            await _hubContext.Clients.Group(movie).NewRatingArrived(command);
            return Ok();
        }

        [HttpGet("{movie}/ratings")]
        public IActionResult Get([FromRoute] string movie)
        {
            return Ok(_repository.GetRatings(movie));
        }
    }
}