namespace ProniaTask.Models
{
    public class Size
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Value { get; set; }
        public List<ProductSize>? ProductSizes { get; set; }
    }
}
