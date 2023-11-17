using Microsoft.AspNetCore.Mvc;
using RTL.TVMazeScraper.API.Models;
using RTL.TVMazeScraper.Storage;
using Show = RTL.TVMazeScraper.API.Models.Show;

namespace RTL.TVMazeScraper.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ShowsAndCastController : ControllerBase
{
    private const int PageSize = 250;

    private readonly ILogger<ShowsAndCastController> _logger;
    private readonly IStorage _storage;

    public ShowsAndCastController(ILogger<ShowsAndCastController> logger, IStorage storage)
    {
        _logger = logger;
        _storage = storage;
    }

    [HttpGet]
    public async Task<IEnumerable<Show>> Get(int pageNumber = 0, int pageSize = PageSize, CancellationToken cancellationToken = default)
    {
        var shows = await _storage.Get<Storage.Models.Show>(pageNumber, pageSize, cancellationToken);
        return shows.Select(x => new Show
        {
            Id = x.Id,
            Name = x.Name,
            Cast = x.Cast.OrderBy(person => person.Birthday).Select(person => new Person
            {
                Id = person.Id,
                Birthday = person.Birthday,
                Name = person.Name
            })
        });

    }
}