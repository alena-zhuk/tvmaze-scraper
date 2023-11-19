namespace RTL.TVMazeScraper.Models
{
    public class Show
    {
        public long Id { get; set; }
        public int LastModifiedTimestamp { get; set; }
        public string Name { get; set; }
        public IEnumerable<Person> Cast { get; set; }

    }
}
