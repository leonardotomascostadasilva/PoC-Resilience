using PoC_Resilience;
using Polly;
using Polly.Retry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddSingleton<WeatherService>();

builder.Services.AddResiliencePipeline("default", x =>
{
    x.AddRetry(new RetryStrategyOptions
    {
        ShouldHandle = new PredicateBuilder().Handle<Exception>(),
        Delay = TimeSpan.FromSeconds(2),
        MaxRetryAttempts = 2,
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        OnRetry = static args =>
        {
            Console.WriteLine("OnRetry, Attempt: {0} - {1}", args.AttemptNumber, args.Outcome.Exception.Message);

            return default;
        }
    })
        .AddTimeout(TimeSpan.FromSeconds(30));
});

var app = builder.Build();

app.MapGet("/weather/{city}", async (string city, WeatherService weatherService) =>
{

    var weather = await weatherService.GetWeatherAsync(city);
    return weather is null ? Results.NotFound() : Results.Ok(weather);
});

app.Run();
