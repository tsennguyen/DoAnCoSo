using System.ComponentModel.DataAnnotations;

namespace Shopping_Laptop.Models
{
    public class CouponModel
    {
        [Key]
        public int Id { get; set; }
        [Required (ErrorMessage =" Yêu Cầu Tên Mã Khuyến Mãi")]
        public string Name { get; set; }
        [Required(ErrorMessage = " Yêu Cầu Tên Mô Tả")]

        public string Description { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateExpired { get; set; }

        [Required(ErrorMessage = " Yêu Cầu Số Lượng Mã Giảm Giá")]

        public int Quantity { get; set; }
        public int Status { get; set; }
    }
}
