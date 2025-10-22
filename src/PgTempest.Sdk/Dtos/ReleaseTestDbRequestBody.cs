using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Dtos;

internal sealed record ReleaseTestDbRequestBody(
    [property: JsonPropertyName("templateHash")] string TemplateHash,
    [property: JsonPropertyName("testDbId")] ushort TestDbId
);
