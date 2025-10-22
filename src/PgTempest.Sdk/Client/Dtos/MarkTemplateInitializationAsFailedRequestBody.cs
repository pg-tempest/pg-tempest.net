using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Client.Dtos;

internal sealed record MarkTemplateInitializationAsFailedRequestBody(
    [property: JsonPropertyName("templateHash")] string TemplateHash
);
