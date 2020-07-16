using System.Threading.Tasks;

namespace SampleApp.Api
{
    public interface IReviewSubscriptions
    {
        Task NewRatingArrived(AddRating rating);
    }
}