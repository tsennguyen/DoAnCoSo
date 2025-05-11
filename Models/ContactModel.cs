using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Laptop.Models
{
    public class ContactModel
    {
        [Key]

        [Required(ErrorMessage = "Yêu cầu nhập Tiêu Đề Website")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Bản Đồ")]
        public string Map { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập SDT")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập thông tin liên hệ")]
        public string Description { get; set; }
        public string LogoImg { get; set; }

        [NotMapped]
        [FileExtensions]
        public IFormFile? ImageUpload { get; set; }



    }
}
