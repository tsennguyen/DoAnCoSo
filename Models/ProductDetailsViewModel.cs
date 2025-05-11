using System.ComponentModel.DataAnnotations;
namespace Shopping_Laptop.Models
{
    public class ProductDetailsViewModel
    {
        public ProductModel ProductDetail { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Đánh giá")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Tên")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Yêu cầu nhập Email")]
        public string Email { get; set; }

    }
}
