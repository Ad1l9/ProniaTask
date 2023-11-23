using System.ComponentModel.DataAnnotations;

namespace ProniaTask.Models
{
    public class Tag
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Bu saheni bos qoymaq olmaz")]
        [MaxLength(25, ErrorMessage = "Uzunlugu 25 xarakterden cox olmamalidir")]
        public string Name { get; set; }
        public List<ProductTag>? ProductTags { get; set; }
    }
}
