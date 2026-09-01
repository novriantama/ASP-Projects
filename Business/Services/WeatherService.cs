using System.Net;
using System.Text.Json;
using ASPProjects.Models.DTOs;

namespace ASPProjects.Business.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<WeatherService> _logger;

    public WeatherService(HttpClient httpClient, IConfiguration configuration, ILogger<WeatherService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<WeatherResponseDto> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City name cannot be empty.", nameof(city));
        }

        var apiKey = _configuration["WeatherAPI_KEY"]
            ?? _configuration["WEATHER_API_KEY"]
            ?? Environment.GetEnvironmentVariable("WeatherAPI_KEY")
            ?? Environment.GetEnvironmentVariable("WEATHER_API_KEY");

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _logger.LogError("WeatherAPI key is missing in configuration or environment variables.");
            throw new InvalidOperationException("Weather service is not properly configured.");
        }

        var requestUri = $"current.json?key={apiKey}&q={Uri.EscapeDataString(city.Trim())}&aqi=no";

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.GetAsync(requestUri, cancellationToken);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // Condition 1: External API Timeout
            _logger.LogWarning(ex, "Request to WeatherAPI timed out for city: {City}", city);
            throw new TimeoutException("The external weather service timed out. Please try again later.");
        }
        catch (HttpRequestException ex)
        {
            // Condition 2: External API Network / Connection error
            _logger.LogError(ex, "Failed to connect to WeatherAPI for city: {City}", city);
            throw new HttpRequestException("Unable to communicate with external weather service.", ex, HttpStatusCode.BadGateway);
        }

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

        // Handle WeatherAPI Error Responses (e.g. 400, 404, 401, 500)
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("WeatherAPI returned status {StatusCode} for city {City}: {Response}",
                response.StatusCode, city, responseContent);

            try
            {
                using var errorDoc = JsonDocument.Parse(responseContent);
                if (errorDoc.RootElement.TryGetProperty("error", out var errorElement))
                {
                    var errorCode = errorElement.TryGetProperty("code", out var codeElem) ? codeElem.GetInt32() : 0;
                    var errorMessage = errorElement.TryGetProperty("message", out var msgElem) ? msgElem.GetString() : string.Empty;

                    // Condition 4: Data Not Found (Code 1006 = "No matching location found")
                    if (errorCode == 1006 || response.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new KeyNotFoundException($"Weather data for location '{city}' was not found.");
                    }

                    // Condition 2: Other External API errors
                    throw new HttpRequestException("External weather provider returned an error.", null, HttpStatusCode.BadGateway);
                }
            }
            catch (JsonException)
            {
                // Unparseable error body
                Console.WriteLine($"WeatherAPI returned an invalid or incomplete response structure for city: {city}");
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Weather data for location '{city}' was not found.");
            }

            throw new HttpRequestException("External weather provider returned an unexpected error.", null, HttpStatusCode.BadGateway);
        }

        // Condition 3: Response validation & deserialization
        try
        {
            var weatherData = JsonSerializer.Deserialize<WeatherResponseDto>(responseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (weatherData?.Location == null || weatherData?.Current == null || string.IsNullOrWhiteSpace(weatherData.Location.Name))
            {
                _logger.LogError("WeatherAPI returned an invalid or incomplete response structure for city: {City}", city);
                throw new FormatException("Received an invalid response format from the external weather provider.");
            }

            return weatherData;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse WeatherAPI JSON response for city: {City}", city);
            throw new FormatException("Failed to process weather response format.", ex);
        }
    }
}
