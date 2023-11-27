using ProniaTask.Models;
using System.ComponentModel.DataAnnotations;

namespace ProniaTask.Areas.ProniaAdmin.ViewModels
{
    public class CreateCategoryVM
    {
        [Required(ErrorMessage = "Bu saheni bos qoymaq olmaz")]
        [MaxLength(25, ErrorMessage = "Uzunlugu 25 xarakterden cox olmamalidir")]
        public string Name { get; set; }
    }
}
