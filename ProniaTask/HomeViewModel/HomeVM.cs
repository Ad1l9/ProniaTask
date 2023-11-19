using ProniaTask.Models;

namespace ProniaTask.HomeViewModel
{
    public class HomeVM
    {
        public List<Product> Products { get; set; }
        public List<Product> Featured { get; set; } 
        public List<Product> Bestseller { get; set; } 
        public List<Product> Latest { get; set; }
    }
}
