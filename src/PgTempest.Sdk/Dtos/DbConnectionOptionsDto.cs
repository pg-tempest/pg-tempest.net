using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Dtos;

internal sealed record DbConnectionOptionsDto(
    [property: JsonPropertyName("host"), JsonRequired] string Host,
    [property: JsonPropertyName("port"), JsonRequired] ushort Port,
    [property: JsonPropertyName("username"), JsonRequired] string Username,
    [property: JsonPropertyName("password"), JsonRequired] string Password,
    [property: JsonPropertyName("database"), JsonRequired] string Database
);
