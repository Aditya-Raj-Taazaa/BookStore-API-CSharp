using System.ComponentModel.DataAnnotations;

namespace Test_API.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public required string Title { get; set; }

        public required int Price { get; set; }

        // Foreign Key
        public int AuthorId { get; set; }
        public  Author? Author { get; set; }
    }
}