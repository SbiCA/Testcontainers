using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using SampleApp;
using SampleApp.Api;
using Xunit;
using Xunit.Abstractions;

namespace Testcontainers
{
    public class End2EndTests : IClassFixture<PostgreSqlFixture>, IClassFixture<RedisFixture>, IDisposable
    {
        private const string MovieId = "It-Crowd";
        private readonly IWebHost _host;
        private readonly HttpClient _httpClient;
        private readonly HubConnection _signalRConnection;
        private readonly ITestOutputHelper _testOutputHelper;

        public End2EndTests(PostgreSqlFixture postgresFixture, RedisFixture redisFixture,
            ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            _host = WebHost.CreateDefaultBuilder()
                        .UseStartup<Startup>()
                        .UseTestServer()
                        .ConfigureTestServices(services =>
                        {
                            // add document store pointing to postgres testcontainer
                            services.AddDocumentStore(postgresFixture.ConnectionString);
                            services.AddSignalR()
                                .AddJsonProtocol()
                                // add redis pointing to testcontainer 
                                .AddStackExchangeRedis(redisFixture.ConnectionString);
                        })
                        .Build();

            _host.Start();
            _httpClient = _host.GetTestClient();
            var httpMessageHandler = _host.GetTestServer().CreateHandler();
            _signalRConnection = new HubConnectionBuilder()
                .AddJsonProtocol()
                .WithUrl($"{_httpClient.BaseAddress}hubs/review",
                    options => { options.HttpMessageHandlerFactory = _ => httpMessageHandler; })
                .Build();
        }

        public void Dispose()
        {
            _host?.Dispose();
        }

        [Fact]
        public async Task GivenSubscriptionToItCrowd_WhenReviewGetsSubmitted_ThenReceivedTheRatingViaSignalR()
        {
            var ratingToSubmit = new AddRating
            {
                User = "sbica",
                Stars = 5
            };
            await GivenSubscription();

            AddRating receivedRating = null;
            var ratingReceived = new AutoResetEvent(false);
            _signalRConnection.On<AddRating>("NewRatingArrived", newRating =>
            {
                _testOutputHelper.WriteLine($"{newRating.User} rated {newRating.Stars}");
                receivedRating = newRating;
                ratingReceived.Set();
            });

            await _httpClient.PostAsJsonAsync($"api/reviews/{MovieId}/ratings", ratingToSubmit);


            ratingReceived.WaitOne(TimeSpan.FromSeconds(2));
            receivedRating.Should().BeEquivalentTo(ratingToSubmit);
        }

        private async Task GivenSubscription()
        {
            await _signalRConnection.StartAsync();
            await _signalRConnection.InvokeAsync("SubscribeMovie", MovieId);
        }
    }
}