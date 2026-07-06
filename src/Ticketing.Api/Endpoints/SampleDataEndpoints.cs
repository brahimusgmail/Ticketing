namespace Ticketing.Api.Endpoints;

using Common.SampleDataGenerator.Abstractions;
using Common.SampleDataGenerator.Helpers;
using Common.SampleDataGenerator.RAG.Abstractions;
using Ticketing.Shared.Contracts.AI;

public static class SampleDataEndpoints
{
    public static IEndpointRouteBuilder MapSampleDataEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/sample-data");

        group.MapPost(
            "/generate",
            async (
                ISampleDataGenerator generator,
                CancellationToken cancellationToken) =>
            {
                var schema = SchemaGenerator.Generate<AiTicketSeedDto>();
                var result = await generator.GenerateAsync<AiTicketSeedDto>(
                    schema,
                    "Generate diverse refresh token records for different scenarios such as mobile" +
                    " login, browser session renewal, remember me, token rotation, password reset, " +
                    "multi-device login, API authentication and security renewal. " +
                    "Each Description must be different.",
                    10,
                    cancellationToken);

                return Results.Ok(result);
            });

        group.MapPost(
            "/ask",
            async (
                RagQuestionRequest request,
                IRagAnswerService ragAnswerService,
                CancellationToken cancellationToken) =>
            {
                var answer = await ragAnswerService.AskAsync(
                    request.Question,
                    cancellationToken);

                return Results.Ok(answer);
            });

        return endpoints;
    }
}
