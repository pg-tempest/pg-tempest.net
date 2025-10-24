using Npgsql;
using PgTempest.Sdk.Factory;

namespace Samples.Dupper.Tests.NUnit;

[Parallelizable(ParallelScope.All)]
[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
internal sealed class CatalogRepositoryTests
{
    private TestDbUsageGuard _testDbUsageGuard;
    private NpgsqlDataSource _dataSource;

    [SetUp]
    public async Task Setup()
    {
        _testDbUsageGuard = await DbFactoryFixture.TestDbFactory.CreateTestDb(
            TimeSpan.FromSeconds(2)
        );

        _dataSource = NpgsqlDataSource.Create(_testDbUsageGuard.ConnectionOptions.ConnectionString);
    }

    [TearDown]
    public async Task TearDown()
    {
        await _testDbUsageGuard.DisposeAsync();
        await _dataSource.DisposeAsync();
    }

    [Test]
    public async Task Foo1() { }

    [Test]
    public async Task Foo2() { }

    [Test]
    public async Task Foo3() { }

    [Test]
    public async Task Foo4() { }

    [Test]
    public async Task Foo5() { }
}
