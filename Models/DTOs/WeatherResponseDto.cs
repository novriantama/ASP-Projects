using System.Text.Json.Serialization;

namespace ASPProjects.Models.DTOs;

public class WeatherResponseDto
{
    [JsonPropertyName("location")]
    public WeatherLocationDto Location { get; set; } = new();

    [JsonPropertyName("current")]
    public WeatherCurrentDto Current { get; set; } = new();
}

public class WeatherLocationDto
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("region")]
    public string Region { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("lat")]
    public double Lat { get; set; }

    [JsonPropertyName("lon")]
    public double Lon { get; set; }

    [JsonPropertyName("tz_id")]
    public string TzId { get; set; } = string.Empty;

    [JsonPropertyName("localtime")]
    public string Localtime { get; set; } = string.Empty;
}

public class WeatherCurrentDto
{
    [JsonPropertyName("last_updated")]
    public string LastUpdated { get; set; } = string.Empty;

    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }

    [JsonPropertyName("temp_f")]
    public double TempF { get; set; }

    [JsonPropertyName("is_day")]
    public int IsDay { get; set; }

    [JsonPropertyName("condition")]
    public WeatherConditionDto Condition { get; set; } = new();

    [JsonPropertyName("wind_kph")]
    public double WindKph { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("feelslike_c")]
    public double FeelslikeC { get; set; }

    [JsonPropertyName("uv")]
    public double Uv { get; set; }
}

public class WeatherConditionDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;

    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;

    [JsonPropertyName("code")]
    public int Code { get; set; }
}
