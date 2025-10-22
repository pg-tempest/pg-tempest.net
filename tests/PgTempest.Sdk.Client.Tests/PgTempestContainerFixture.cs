using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;

namespace PgTempest.Sdk.Client.Tests;

[SetUpFixture]
public static class PgTempestContainerFixture
{
    public const string PgUser = "pg-username";
    public const string PgPassword = "pg-password";

    public static INetwork Network;
    public static PostgreSqlContainer PostgreSqlContainer;
    public static IContainer PgTempestContainer;

    public static HttpClient GetPgTempestHttpClient()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(
            $"http://localhost:{PgTempestContainer.GetMappedPublicPort(8000)}"
        );

        return httpClient;
    }

    [OneTimeSetUp]
    public static async Task Setup()
    {
        Network = new NetworkBuilder().WithCleanUp(true).Build();

        PostgreSqlContainer = new PostgreSqlBuilder()
            .WithUsername(PgUser)
            .WithPassword(PgPassword)
            .WithNetwork(Network)
            .Build();

        await PostgreSqlContainer.StartAsync();

        PgTempestContainer = new ContainerBuilder()
            .WithImage("pg-tempest:0.1.0-alpha")
            .WithPortBinding(8000, true)
            .WithNetwork(Network)
            .WithEnvironment("PG_TEMPEST_CORE_DBMS_USER", PgUser)
            .WithEnvironment("PG_TEMPEST_CORE_DBMS_PASSWORD", PgPassword)
            .WithEnvironment("PG_TEMPEST_CORE_DBMS_INNER_HOST", PostgreSqlContainer.IpAddress)
            .WithEnvironment(
                "PG_TEMPEST_CORE_DBMS_INNER_PORT",
                PostgreSqlBuilder.PostgreSqlPort.ToString()
            )
            .WithEnvironment("PG_TEMPEST_CORE_DBMS_OUTER_HOST", "localhost")
            .WithEnvironment(
                "PG_TEMPEST_CORE_DBMS_OUTER_PORT",
                PostgreSqlContainer.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort).ToString()
            )
            .Build();

        await PgTempestContainer.StartAsync();
    }

    [OneTimeTearDown]
    public static async Task TearDown()
    {
        await Network.DisposeAsync();
        await PostgreSqlContainer.DisposeAsync();
        await PgTempestContainer.DisposeAsync();
    }
}
