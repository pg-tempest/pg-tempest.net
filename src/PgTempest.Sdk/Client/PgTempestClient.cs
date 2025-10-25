using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using PgTempest.Sdk.Client.Dtos;
using PgTempest.Sdk.Client.Models;
using PgTempest.Sdk.Models;
using StartTemplateInitializationResult = PgTempest.Sdk.Client.Models.StartTemplateInitializationResult;

namespace PgTempest.Sdk.Client;

public sealed class PgTempestClient(HttpClient httpClient)
{
    public static PgTempestClient NewFromBaseUrl([StringSyntax("Uri")] string baseUrl)
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(baseUrl);

        return new PgTempestClient(httpClient);
    }

    public async Task<StartTemplateInitializationResult> StartTemplateInitialization(
        TemplateHash templateHash,
        TimeSpan initializationDuration,
        CancellationToken cancellationToken = default
    )
    {
        if (initializationDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(initializationDuration),
                $"{nameof(initializationDuration)} must be positive"
            );
        }

        var request = new StartTemplateInitializationRequestBody(
            templateHash.ToString(),
            InitializationDurationMs: (ulong)initializationDuration.TotalMilliseconds
        );
        var httpResponse = await httpClient.PostAsJsonAsync(
            "/api/start-template-initialization",
            request,
            cancellationToken
        );
        var responseBody =
            await httpResponse.Content.ReadFromJsonAsync<StartTemplateInitializationResponseBody>(
                cancellationToken
            );

        return responseBody switch
        {
            { InitializationWasStarted: { } started } =>
                new StartTemplateInitializationResult.InitializationWasStarted(
                    DbConnectionOptions: new DbConnectionOptions(
                        started.DatabaseConnectionOptions.Host,
                        started.DatabaseConnectionOptions.Port,
                        started.DatabaseConnectionOptions.Username,
                        started.DatabaseConnectionOptions.Password,
                        started.DatabaseConnectionOptions.Database
                    ),
                    InitializationDeadline: started.InitializationDeadline
                ),
            { InitializationIsInProgress: { } inProgress } =>
                new StartTemplateInitializationResult.InitializationIsInProgress(
                    InitializationDeadline: inProgress.InitializationDeadline
                ),
            { InitializationIsFinished: { } } =>
                new StartTemplateInitializationResult.InitializationIsFinished(),
            { UnexpectedError.Message: var message } => throw new Exception(message),
            _ => throw new NullReferenceException(),
        };
    }

    public async Task FinishTemplateInitialization(
        TemplateHash templateHash,
        CancellationToken cancellationToken = default
    )
    {
        var request = new FinishTemplateInitializationRequestBody(templateHash.ToString());
        var httpResponse = await httpClient.PostAsJsonAsync(
            "/api/finish-template-initialization",
            request,
            cancellationToken
        );

        var responseBody =
            await httpResponse.Content.ReadFromJsonAsync<FinishTemplateInitializationResponseBody>(
                cancellationToken
            );

        switch (responseBody)
        {
            case { InitializationIsFinished: { } }:
                return;
            case { InitializationIsFailed: { } }:
                throw new InvalidOperationException("Initialization is failed");
            case { TemplateWasNotFound: { } }:
                throw new InvalidOperationException("Template was not found");
            default:
                throw new NullReferenceException();
        }
    }

    public async Task FailTemplateInitialization(
        TemplateHash templateHash,
        CancellationToken cancellationToken = default
    )
    {
        var request = new FailTemplateInitializationRequestBody(templateHash.ToString());

        var httpResponse = await httpClient.PostAsJsonAsync(
            "/api/fail-template-initialization",
            request,
            cancellationToken
        );

        var responseBody =
            await httpResponse.Content.ReadFromJsonAsync<FailTemplateInitializationResponseBody>(
                cancellationToken
            );

        switch (responseBody)
        {
            case { InitializationIsFailed: { } }:
                return;
            case { InitializationIsFinished: { } }:
                throw new InvalidOperationException("Initialization is finished");
            case { TemplateWasNotFound: { } }:
                throw new InvalidOperationException("Template was not found");
            default:
                throw new NullReferenceException();
        }
    }

    public async Task<GetTestDbResult> GetTestDb(
        TemplateHash templateHash,
        TimeSpan usageDuration,
        CancellationToken cancellationToken = default
    )
    {
        if (usageDuration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(usageDuration),
                $"{nameof(usageDuration)} must be positive"
            );
        }

        var request = new GetTestDbRequestBody(
            templateHash.ToString(),
            UsageDurationMs: (ulong)usageDuration.TotalMilliseconds
        );

        var httpResponse = await httpClient.PostAsJsonAsync(
            "/api/get-test-db",
            request,
            cancellationToken
        );

        var responseBody = await httpResponse.Content.ReadFromJsonAsync<GetTestDbResponseBody>(
            cancellationToken
        );

        switch (responseBody)
        {
            case { TestDbWasCreated: { } testDbWasCreated }:
                return new GetTestDbResult(
                    TestDbId.Parse(testDbWasCreated.TestDbId),
                    new DbConnectionOptions(
                        testDbWasCreated.DbConnectionOptions.Host,
                        testDbWasCreated.DbConnectionOptions.Port,
                        testDbWasCreated.DbConnectionOptions.Username,
                        testDbWasCreated.DbConnectionOptions.Password,
                        testDbWasCreated.DbConnectionOptions.Database
                    ),
                    testDbWasCreated.UsageDeadline
                );
            case { TemplateIsNotInitialized: { } }:
                throw new InvalidOperationException("Template is not initialized");
            case { TemplateWasNotFound: { } }:
                throw new InvalidOperationException("Template was not found");
            case { UnknownError.Message: { } message }:
                throw new Exception(message);
            default:
                throw new NullReferenceException();
        }
    }

    public async Task FinishTestDbUsage(
        TemplateHash templateHash,
        TestDbId testDbId,
        CancellationToken cancellationToken = default
    )
    {
        var request = new FinishTestDbUsageRequestBody(
            templateHash.ToString(),
            testDbId.ToString()
        );

        var httpResponse = await httpClient.PostAsJsonAsync(
            "/api/finish-test-db-usage",
            request,
            cancellationToken
        );

        var responseBody =
            await httpResponse.Content.ReadFromJsonAsync<FinishTestDbUsageResponseBody>(
                cancellationToken
            );

        switch (responseBody)
        {
            case { TestDbWasReleased: { } }:
                return;
            case { TemplateWasNotFound: { } }:
                throw new InvalidOperationException("Template was not found");
            case { TestDbWasNotFound: { } }:
                throw new InvalidOperationException("Test db was not found");
            case { TestDbIsNotUsed: { } }:
                throw new InvalidOperationException("Test db is not used");
            default:
                throw new NullReferenceException();
        }
    }

    public async Task InitializeTemplate(
        TemplateHash templateHash,
        TimeSpan initializationDuration,
        Func<DbConnectionOptions, Task> initializationCallback,
        CancellationToken cancellationToken = default
    )
    {
        StartTemplateInitializationResult startResult;
        while (true)
        {
            startResult = await StartTemplateInitialization(
                templateHash,
                initializationDuration,
                cancellationToken
            );

            if (startResult is StartTemplateInitializationResult.InitializationIsInProgress)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
                continue;
            }

            break;
        }

        switch (startResult)
        {
            case StartTemplateInitializationResult.InitializationIsFinished:
            {
                return;
            }
            case StartTemplateInitializationResult.InitializationWasStarted started:
            {
                try
                {
                    await initializationCallback(started.DbConnectionOptions);
                    await FinishTemplateInitialization(templateHash, cancellationToken);
                }
                catch
                {
                    await FailTemplateInitialization(templateHash, CancellationToken.None);
                    throw;
                }
                return;
            }
        }
    }
}
