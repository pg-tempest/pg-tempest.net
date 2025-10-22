using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Dtos;

internal sealed record FinishTemplateInitializationRequestBody(
    [property: JsonPropertyName("templateHash")] string TemplateHash
);
