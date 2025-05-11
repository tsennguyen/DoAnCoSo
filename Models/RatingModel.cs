using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Laptop.Models
{
    public class RatingModel
    {
        [Key]
        public int Id { get; set; }
      
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Yeu Cau Nhap Danh Gia San Pham ")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Yeu Cau Nhap Ten ")]
        public int Name { get; set; }
        [Required(ErrorMessage = "Yeu Cau Nhap Email ")]

        public string Email { get; set; }
        public string Star { get; set; }
        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
