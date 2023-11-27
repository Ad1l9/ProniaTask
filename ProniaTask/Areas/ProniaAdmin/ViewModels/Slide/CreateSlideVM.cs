using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProniaTask.Areas.ProniaAdmin.ViewModels
{
    public class CreateSlideVM
    {
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        [Required]
        public IFormFile Photo { get; set; }
    }
}
