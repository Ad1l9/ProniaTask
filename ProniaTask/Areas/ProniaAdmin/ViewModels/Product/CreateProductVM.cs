using System.ComponentModel.DataAnnotations;

namespace ProniaTask.Areas.ProniaAdmin.ViewModels

{
    public class CreateProductVM
    {
        public string Name { get; set; }
        public float Price { get; set; }
        public int SellCount { get; set; }

        public string SKU { get; set; }

        public string Description { get; set; }

        [Required]
        public int? CategoryId { get; set; }

    }
}
