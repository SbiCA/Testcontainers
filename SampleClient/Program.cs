using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using SampleApp;
using SampleApp.Api;

namespace SampleClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client  = new HubConnectionBuilder()
                .AddJsonProtocol()
                .WithAutomaticReconnect()
                .WithUrl("http://localhost:5000/hubs/review")
                .Build();

            client.On<AddRating>("NewRatingArrived", (newRating) =>
            {
                  Console.WriteLine($"{newRating.User} rated {newRating.Stars}");  
            });

            await client.StartAsync();
            await client.InvokeAsync("SubscribeMovie", "test");

            Console.ReadKey();
        }
    }
}