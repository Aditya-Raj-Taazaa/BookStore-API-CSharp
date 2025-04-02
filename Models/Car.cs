using System.ComponentModel.DataAnnotations;

namespace Test_API.Models
{    
        public class Car
        {
        [Key]
        public int Id { get; set; }
            public required string Make { get; set; }
            public required string Model { get; set; }
            public int Year { get; set; }
        }
}
