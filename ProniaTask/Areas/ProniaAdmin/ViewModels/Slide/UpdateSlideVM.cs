using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProniaTask.Areas.ProniaAdmin.ViewModels
{
    public class UpdateSlideVM
    {
        [Required]
        [MaxLength(25, ErrorMessage = "Max length is 25")]
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public int Order { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
