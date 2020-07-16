using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Xunit;

namespace Testcontainers
{
    public class RedisFixture : IAsyncLifetime
    {
        private readonly RedisTestcontainer _redisContainer;

        public RedisFixture()
        {
            _redisContainer = new TestcontainersBuilder<RedisTestcontainer>()
                .WithCleanUp(true)
                .WithDatabase(new RedisTestcontainerConfiguration())
                .Build();
        }

        public string ConnectionString => _redisContainer.ConnectionString;

        public Task InitializeAsync()
        {
            return _redisContainer.StartAsync();
        }

        public Task DisposeAsync()
        {
            return _redisContainer.StopAsync();
        }
    }
}