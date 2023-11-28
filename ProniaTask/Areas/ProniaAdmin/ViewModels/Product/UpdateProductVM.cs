using ProniaTask.Models;

namespace ProniaTask.Areas.ProniaAdmin.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public int SellCount { get; set; }

        public string SKU { get; set; }

        public string Description { get; set; }
        public int CategoryId { get; set; }


        public List<Category>? Categories { get; set; }
        public List<int> TagIds { get; set; }
        public List<int> ColorIds { get; set; }
        public List<int> SizeIds { get; set; }
        public List<Tag>? Tags { get; set; }
        public List<Color>? Colors { get; set; }
        public List<Size>? Sizes { get; set; }
    }
}
