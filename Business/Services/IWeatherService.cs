using ASPProjects.Models.DTOs;

namespace ASPProjects.Business.Services;

public interface IWeatherService
{
    Task<WeatherResponseDto> GetCurrentWeatherAsync(string city, CancellationToken cancellationToken = default);
}
