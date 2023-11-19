using System.Text.Json.Serialization;

namespace RTL.TVMazeScraper.WorkerService
{

    public class TvMazeShow
    {
        public ulong Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Language { get; set; }
        public List<string> Genres { get; set; }
        public string Status { get; set; }
        public int? Runtime { get; set; }
        public int? AverageRuntime { get; set; }
        public string Premiered { get; set; }
        public string Ended { get; set; }
        public string OfficialSite { get; set; }
        public Schedule Schedule { get; set; }
        public Rating Rating { get; set; }
        public string Summary { get; set; }
        public int Updated { get; set; }

        [JsonPropertyName("_embedded")]
        public Embedded Embedded { get; set; }
    }

    public class Cast
    {
        public TvMazePerson Person { get; set; }
        public Character Character { get; set; }
        public bool Self { get; set; }
        public bool Voice { get; set; }
    }
    public class TvMazePerson
    {
        public ulong Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public Country Country { get; set; }
        public DateTime? Birthday { get; set; }
        public DateTime? Deathday { get; set; }
        public string Gender { get; set; }
        public int Updated { get; set; }
    }

    public class Character
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
    }

    public class Country
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Timezone { get; set; }
    }

    public class Embedded
    {
        public List<Cast> Cast { get; set; }
    }

    public class Schedule
    {
        public string Time { get; set; }
        public List<string> Days { get; set; }
    }
    
    public class Rating
    {
        public double? Average { get; set; }
    }


}
