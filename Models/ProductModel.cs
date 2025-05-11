using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shopping_Laptop.Repository.Validation;

namespace Shopping_Laptop.Models
{
    public class ProductModel
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập Tên Sản phẩm")]
        public string Name { get; set; }

        public string Slug { get; set; }

        [Required, MinLength(4, ErrorMessage = "Yêu cầu nhập Mô tả Sản phẩm")]
        public string Description { get; set; }

        // Removed the Range attribute to allow any value for Price
        [Required(ErrorMessage = "Yêu cầu nhập Giá Sản phẩm")]
        [Column(TypeName = "decimal(20,0)")]

        [DisplayFormat(DataFormatString = "{0:#,0} VND", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Yêu cầu một Thương hiệu")]
        public int BrandId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Yêu cầu một Danh mục")]
        public int CategoryId { get; set; }

        public CategoryModel Category { get; set; }
        public BrandModel Brand { get; set; }
        public RatingModel Ratings { get; set; }
        public string Image { get; set; } = "noimage.png";

        public int Quantity { get; set; }

        public int Sold { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }
    }
}
