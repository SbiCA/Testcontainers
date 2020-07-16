using System.Threading.Tasks;
using DotNet.Testcontainers.Containers.Builders;
using DotNet.Testcontainers.Containers.Configurations.Databases;
using DotNet.Testcontainers.Containers.Modules.Databases;
using Xunit;

namespace Testcontainers
{
    public class PostgreSqlFixture : IAsyncLifetime
    {
        private readonly PostgreSqlTestcontainer _testContainer;

        public PostgreSqlFixture()
        {
            var testContainerBuilder = new TestcontainersBuilder<PostgreSqlTestcontainer>()
                .WithCleanUp(true) // remove container when fixture/test get's disposed
                .WithDatabase(new PostgreSqlTestcontainerConfiguration
                {
                    Database = "postgres",
                    Username = "postgres",
                    Password = "postgres"
                })
                .WithImage("clkao/postgres-plv8"); // use a different base image including plv8 plugin

            _testContainer = testContainerBuilder.Build();
        }

        public string ConnectionString => _testContainer.ConnectionString;

        public async Task InitializeAsync()
        {
            await _testContainer.StartAsync();

            // https://github.com/clkao/docker-postgres-plv8
            // this way you can enable plv8 for the fancy features of marten 🤩
            var result = await _testContainer.ExecAsync(new[]
            {
                "/bin/sh", "-c",
                "psql -U postgres -c \"CREATE EXTENSION plv8; SELECT extversion FROM pg_extension WHERE extname = 'plv8';\""
            });
        }

        public async Task DisposeAsync()
        {
            await _testContainer.StopAsync();
        }
    }
}