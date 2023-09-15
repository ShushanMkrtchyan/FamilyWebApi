using System.Globalization;

namespace MyProjectWebApi.Models.Entities
{
    public class Family:EntityBase
    {
        public string? Title { get; set; }
        public string? Address { get; set; }

        public ICollection<Person> Persons { get; set; }

        public ICollection<Animal> Animals { get; set; }

    }
}
