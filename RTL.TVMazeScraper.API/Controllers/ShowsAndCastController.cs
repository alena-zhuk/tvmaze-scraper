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
    public async Task<ActionResult<IEnumerable<Show>>> Get([FromQuery] int pageNumber = 0, [FromQuery] int pageSize = PageSize, CancellationToken cancellationToken = default)
    {
        if (pageNumber < 0 || pageSize < 0)
            return BadRequest();

        var shows = await _storage.Get<RTL.TVMazeScraper.Models.Show>(pageNumber, pageSize, cancellationToken);
        return new OkObjectResult(
            shows.Select(x => new Show
            {
                Id = x.Id,
                Name = x.Name,
                Cast = x.Cast.OrderByDescending(person => person.Birthday).Select(person => new Person
                {
                    Id = person.Id,
                    Birthday = person.Birthday,
                    Name = person.Name
                })
            }));

    }
}