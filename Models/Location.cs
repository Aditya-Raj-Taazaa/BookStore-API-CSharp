namespace Test_API.Models
{
    public class Location
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Duration { get; set; }
        public required string Distance {get; set;}
        public required string Weather {get; set;}
        public required int Price {get; set;}
        public required string ShortDescription {get; set;}
        public required string LongDescription {get; set;}
    }
}