namespace DDDProject.Domain.Entities
{
    public class Logging
    {
        public int Id { get; set; }
        public string? RequestMethod { get; set; } 
        public string? RequestPath { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseStatusCode { get; set; } 
        public DateTime Timestamp { get; set; }
        public string? ResponseBody { get; set; }
        public string? UserName { get; set; }

    }
}
