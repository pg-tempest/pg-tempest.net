namespace PgTempest.Sdk.Models;

public sealed record DbConnectionOptions(
    string Host,
    ushort Port,
    string Username,
    string Password,
    string Database
)
{
    public string ConnectionString =>
        $"Host={Host};Port={Port};Username={Username};Database={Database};Password={Password};";
}
