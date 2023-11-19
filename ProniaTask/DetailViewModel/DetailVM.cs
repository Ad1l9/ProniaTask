using ProniaTask.Models;
using System.Reflection.Metadata.Ecma335;

namespace ProniaTask.DetailViewModel
{
    public class DetailVM
    {
        public List<Product> RelatedProducts { get; set; }
        public Product Product { get; set; }
    }
}
