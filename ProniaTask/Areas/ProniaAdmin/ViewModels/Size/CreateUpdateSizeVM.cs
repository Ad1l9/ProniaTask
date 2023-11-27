using System.ComponentModel.DataAnnotations;

namespace ProniaTask.Areas.ProniaAdmin.ViewModels
{
    public class CreateUpdateSizeVM
    {
        [Required]
        public string Name { get; set; }

    }
}
