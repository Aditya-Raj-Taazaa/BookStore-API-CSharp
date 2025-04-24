namespace Test_API.Models
{
    public class Author
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Bio { get; set; }
        public ICollection<Book> Books { get; set; } = [];
    }
}
