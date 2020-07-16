using Marten;
using Marten.Services;
using Microsoft.Extensions.DependencyInjection;

namespace SampleApp
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDocumentStore(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<IDocumentStore>(GetDocumentStore(connectionString));
        }

        public static DocumentStore GetDocumentStore(string connectionString)
        {
            return DocumentStore.For(options =>
            {
                options.AutoCreateSchemaObjects = AutoCreate.All;
                options.Serializer(new JsonNetSerializer
                {
                    EnumStorage = EnumStorage.AsString
                });
                options.Schema.For<Review>().Identity(i => i.Movie);
                options.Connection(connectionString);
            });
        }
    }
}