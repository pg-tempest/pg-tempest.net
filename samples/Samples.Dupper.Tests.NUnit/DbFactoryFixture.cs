using Npgsql;
using PgTempest.Sdk.Client;
using PgTempest.Sdk.Factory;
using PgTempest.Sdk.Models;

namespace Samples.Dupper.Tests.NUnit;

[SetUpFixture]
internal sealed class DbFactoryFixture
{
    public const string PgTempestHost = "127.0.0.1";
    public const int PgTempestPort = 8000;

    public static TestDbFactory TestDbFactory { get; private set; }

    [OneTimeSetUp]
    public static async Task Setup()
    {
        var templateHash = TemplateHash.Calculate(
            Migrator.GetMigrations().Select(x => x.Name + x.Content)
        );

        var pgTempestClient = PgTempestClient.NewFromBaseUrl(
            $"http://{PgTempestHost}:{PgTempestPort}"
        );

        await pgTempestClient.InitializeTemplate(
            templateHash,
            TimeSpan.FromSeconds(30),
            async dbConnectionOptions =>
            {
                await using var npgsqlDataSource = NpgsqlDataSource.Create(
                    dbConnectionOptions.ConnectionString
                );

                var migrator = new Migrator(npgsqlDataSource);
                await migrator.MigrateAsync();
            }
        );

        TestDbFactory = new TestDbFactory(pgTempestClient, templateHash);
    }
}
