using Dapper;
using Npgsql;

namespace Samples.Dupper;

public sealed class Migrator(NpgsqlDataSource dataSource)
{
    public async Task MigrateAsync()
    {
        await using var connection = await dataSource.OpenConnectionAsync();

        await CreateMigrationsTableIfNotExists(connection);

        var appliedMigrations = await GetAppliedMigrations(connection);

        var migrations = GetMigrations()
            .ExceptBy(appliedMigrations, x => x.Name)
            .OrderBy(x => x.Name);

        foreach (var (migrationName, migrationContent) in migrations)
        {
            try
            {
                await connection.ExecuteAsync(migrationContent);
                await AddAppliedMigrations(connection, migrationName);
            }
            catch (Exception exception)
            {
                throw new Exception($"Migration {migrationName} failed", exception);
            }
        }
    }

    public static IEnumerable<(string Name, string Content)> GetMigrations()
    {
        var files = Directory.GetFiles("./Migrations");

        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            var content = File.ReadAllText(file);

            yield return (name, content);
        }
    }

    private async Task CreateMigrationsTableIfNotExists(NpgsqlConnection connection)
    {
        await connection.ExecuteAsync(
            """
            CREATE TABLE IF NOT EXISTS __applied_migrations
            (
                name text NOT NULL
            );
            """
        );
    }

    private async Task AddAppliedMigrations(NpgsqlConnection connection, string migrationName)
    {
        await connection.ExecuteAsync(
            """
            INSERT INTO __applied_migrations (name)
            VALUES (@name)
            """,
            new { name = migrationName }
        );
    }

    private async Task<IEnumerable<string>> GetAppliedMigrations(NpgsqlConnection connection)
    {
        return await connection.QueryAsync<string>(
            """
            SELECT name 
            FROM __applied_migrations
            """
        );
    }
}
