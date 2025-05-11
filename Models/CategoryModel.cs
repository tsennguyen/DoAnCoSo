using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Laptop.Models
{
    public class CategoryModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Đảm bảo ID tự động tăng
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên danh mục không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mô tả danh mục")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá 500 ký tự")]
        public string Description { get; set; }

       
        public string Slug { get; set; }

        [Required(ErrorMessage = "Trạng thái danh mục là bắt buộc")]
        [Range(0, 1, ErrorMessage = "Trạng thái phải là 0 (không hoạt động) hoặc 1 (hoạt động)")]
        public int Status { get; set; }
    }
}
