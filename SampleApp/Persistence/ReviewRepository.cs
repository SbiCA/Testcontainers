using System.Collections.Generic;
using System.Threading.Tasks;
using Marten;

namespace SampleApp.Persistence
{
    public class ReviewRepository
    {
        private readonly IDocumentStore _documentStore;

        public ReviewRepository(IDocumentStore documentStore)
        {
            _documentStore = documentStore;
        }

        public async Task AddRating(string movie, string user, int stars)
        {
            using var session = _documentStore.LightweightSession();
            if (session.Load<Review>(movie) == null)
                // init document if non existent ... can of course be solved differently 
                session.Store(new Review
                {
                    Movie = movie
                });

            // Patch is using plv8 features to use a javascript function.
            session
                .Patch<Review>(movie)
                .Append(r => r.Ratings, new Rating
                {
                    Stars = stars,
                    User = user
                });
            await session.SaveChangesAsync();
        }

        public IEnumerable<Rating> GetRatings(string movie)
        {
            using var session = _documentStore.LightweightSession();
            return session.Load<Review>(movie)?.Ratings ?? new List<Rating>();
        }
    }
}