using ProniaTask.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace ProniaTask.ViewModel
{
    public class RegisterVM
    {

        [Required]
        [MinLength(4, ErrorMessage = "Username uzunlugu 4den boyuk olmalidir")]
        [MaxLength(25, ErrorMessage = "Username 25 simvoldan cox ola bilmez")]
        public string Username { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Name uzunlugu 3den boyuk olmalidir")]
        [MaxLength(25, ErrorMessage = "Name 25 simvoldan cox ola bilmez")]
        public string Name { get; set; }
        public Gender Gender { get; set; }

        [Required]
        [MinLength(4, ErrorMessage = "Surname uzunlugu 3den boyuk olmalidir")]
        [MaxLength(25, ErrorMessage = "Surname 25 simvoldan cox ola bilmez")]
        public string Surname { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [MinLength(10, ErrorMessage = "Email uzunlugu 10dan boyuk olmalidir")]
        [MaxLength(256, ErrorMessage = "Email 256 simvoldan cox ola bilmez")]
        [RegularExpression("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z]{2,}(?:\\.[a-zA-Z]{2,})+$",
        ErrorMessage = "Email duzgun formatda deyil")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
