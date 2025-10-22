using System.Text.Json.Serialization;

namespace PgTempest.Sdk.Client.Dtos;

internal sealed record GetTestDbResponseBody(
    [property: JsonPropertyName("testDbWasCreated")]
        GetTestDbResponseBodyTestDbWasCreated? TestDbWasCreated,
    [property: JsonPropertyName("templateWasNotFound")] EmptyDto? TemplateWasNotFound,
    [property: JsonPropertyName("templateIsNotInitialized")] EmptyDto? TemplateIsNotInitialized,
    [property: JsonPropertyName("unknownError")] GetTestDbResponseBodyUnknownError? UnknownError
);

internal sealed record GetTestDbResponseBodyTestDbWasCreated(
    [property: JsonPropertyName("testDbId"), JsonRequired] ushort TestDbId,
    [property: JsonPropertyName("dbConnectionOptions"), JsonRequired]
        DbConnectionOptionsDto DbConnectionOptions,
    [property: JsonPropertyName("usageDeadline"), JsonRequired] DateTime UsageDeadline
);

internal sealed record GetTestDbResponseBodyUnknownError(
    [property: JsonPropertyName("message"), JsonRequired] string Message
);
