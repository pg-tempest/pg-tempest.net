namespace PgTempest.Sdk.Client.Models;

public sealed record DbConnectionOptions(
    string Host,
    ushort Port,
    string Username,
    string Password,
    string Database
);
