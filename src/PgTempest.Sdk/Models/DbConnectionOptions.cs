namespace PgTempest.Sdk.Models;

public sealed record DbConnectionOptions(
    string Host,
    ushort Port,
    string Username,
    string Password,
    string Database
);
