using System.ComponentModel.DataAnnotations;

namespace ProniaTask.Areas.ProniaAdmin.ViewModels
{
    public class UpdateCreateColorVM
    {
        [Required]
        public string Name { get; set; }
    }
}
