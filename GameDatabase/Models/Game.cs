namespace GameDatabase.Models;

    public class Game
    {
        public int Id { get; set; }
        public int? Rank { get; set; }
        public string? Title { get; set; }
        public string? Platform { get; set; }
        public int? ReleaseYear { get; set; }
        public string? Genre { get; set; }
        public string? Developer { get; set; }
        public int? DeveloperId { get; set; }
    
    }
