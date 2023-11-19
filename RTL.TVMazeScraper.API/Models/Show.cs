namespace RTL.TVMazeScraper.API.Models;

public class Show
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<Person> Cast { get; set; }
    
}