var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", async (ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory) =>
{
    var logger = loggerFactory.CreateLogger("oneday");
    logger.LogInformation("Request received");

    try
    {
        var client = httpClientFactory.CreateClient();
        var url = "http://localhost:3500/v1.0/invoke/forecast/method/forecast";
        var forecast = await client.GetFromJsonAsync<List<WeatherForecast>>(url);

        logger.LogInformation($"Received {forecast?.Count} forecasts");

        return forecast?.FirstOrDefault();
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while processing the request");
        throw;
    }
})
.WithName("GetWeatherForecastForOneDay");

app.Run();

record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}