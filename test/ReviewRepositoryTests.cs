using System.Linq;
using System.Threading.Tasks;
using Marten;
using SampleApp;
using SampleApp.Persistence;
using Xunit;

namespace Testcontainers
{
    public class ReviewRepositoryTests : IClassFixture<PostgreSqlFixture>
    {
        private const string MovieId = "The It crowd - yesterday's jam";
        private readonly DocumentStore _documentStore;

        public ReviewRepositoryTests(PostgreSqlFixture postgreSql)
        {
            _documentStore = ServiceCollectionExtensions.GetDocumentStore(postgreSql.ConnectionString);
        }

        [Fact]
        public async Task GivenLastJedi_WhenAddReview_ThenReviewAndMovieGotAdded()
        {
            GivenYesterdaysJam();

            var sut = new ReviewRepository(_documentStore);

            await sut.AddRating(MovieId, "me", 3);

            using var session = _documentStore.LightweightSession();
            var reviews = await session.LoadAsync<Review>(MovieId);
            var rating = reviews.Ratings.First();
            Assert.Equal(3, rating.Stars);
            Assert.Equal("me", rating.User);
        }

        private void GivenYesterdaysJam()
        {
            using var session = _documentStore.OpenSession();
            session.Store(new Review
            {
                Movie = MovieId
            });
            session.SaveChanges();
        }
    }
}