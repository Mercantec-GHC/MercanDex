using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    namespace API.Models
    {
        public class Pokedex
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Art { get; set; }
            public int? Hp { get; set; }
            public int? Attack { get; set; }
            public int? Defense { get; set; }
            public int? Speed { get; set; }
            public int? Weight { get; set; }
            public int? Height { get; set; }
            public string Description { get; set; }
            public string ImageUrl { get; set; }
        }

        public class PokedexDTO
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Art { get; set; }
            public int? Hp { get; set; }
            public int? Attack { get; set; }
            public int? Defense { get; set; }
            public int? Speed { get; set; }
            public int? Weight { get; set; }
            public int? Height { get; set; }
            public string Description { get; set; }
            public IFormFile ProfilePicture { get; set; }
        }
    }

}
