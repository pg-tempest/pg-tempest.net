using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Client.Dtos;

internal sealed record FinishTestDbUsageRequestBody(
    [property: JsonPropertyName("templateHash")] string TemplateHash,
    [property: JsonPropertyName("testDbId")] string TestDbId
);
