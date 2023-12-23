namespace GameDatabase.Models;

    public class Developer
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public string? Country { get; set; }

        public List<Game> Games { get; set; } = new List<Game>();
    }
