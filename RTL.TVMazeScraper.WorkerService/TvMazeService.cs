using System.Net.Http.Json;
using System.Text.Json;
using RTL.TVMazeScraper.Models;

namespace RTL.TVMazeScraper.WorkerService
{
    public interface ITVMazeService
    {
        Task<Show?> GetShowWithCast(ulong id, CancellationToken cancellationToken);
        Task<IEnumerable<ulong>> GetUpdatedShows(DateTimeOffset? since, CancellationToken cancellationToken);
    }

    public class TvMazeService : ITVMazeService
    {
        private readonly HttpClient _httpClient;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        const string BaseUrl = "https://api.tvmaze.com";

        public TvMazeService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(BaseUrl);
        }
        public async Task<Show?> GetShowWithCast(ulong id, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync($"/shows/{id}?embed=cast", cancellationToken);
            response.EnsureSuccessStatusCode(); // todo add error handling besides the retry backoff policy
            var tvMazeShow = await response.Content.ReadFromJsonAsync<TvMazeShow>(_jsonSerializerOptions, cancellationToken);
            if (tvMazeShow == null)
                return null; // todo: throw a not found exception?

            return new Show
            {
                Id = tvMazeShow.Id,
                Name = tvMazeShow.Name,
                LastModifiedTimestamp =
                    tvMazeShow.Updated, // in future this field can be used to validate if we have the latest or not before doing the api call
                Cast = tvMazeShow.Embedded?.Cast
                           .Select(cast => new Person
                           {
                               Id = cast.Person.Id,
                               Name = cast.Person.Name,
                               Birthday = cast.Person.Birthday
                           })
                       ?? Enumerable.Empty<Person>()
            };
        }

        public async Task<IEnumerable<ulong>> GetUpdatedShows(DateTimeOffset? since,
        CancellationToken cancellationToken)
        {
            var sinceQuery = string.Empty; // get all shows
            if (since != null)
            {
                if (DateTimeOffset.UtcNow <= since.Value.AddDays(1))
                {
                    sinceQuery = "?since=day";
                }
                else if (DateTimeOffset.UtcNow <= since.Value.AddDays(7))
                {
                    sinceQuery = "?since=week";
                }
                else if (DateTimeOffset.UtcNow <= since.Value.AddMonths(1))
                {
                    sinceQuery = "?since=month";
                }
            }

            var response = await _httpClient.GetAsync($"updates/shows{sinceQuery}", cancellationToken);
            response.EnsureSuccessStatusCode();// todo add error handling besides the retry backoff policy
            var updatedShows = await response.Content.ReadFromJsonAsync<Dictionary<ulong, int>>(_jsonSerializerOptions, cancellationToken);
            return updatedShows?.Select(x => x.Key) ?? Enumerable.Empty<ulong>();
        }
    }
}
