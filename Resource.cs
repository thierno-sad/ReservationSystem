namespace ResourcesService.Models
{
    public class Resource
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public decimal PricePerDay { get; set; }
        public bool IsAvailable { get; set; }
    }
}
