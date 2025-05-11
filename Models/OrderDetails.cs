using System.ComponentModel.DataAnnotations.Schema;

namespace Shopping_Laptop.Models
{
    public class OrderDetails
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string OrderCode { get; set; }

        // Change from long to int to match ProductModel.Id
        public int ProductId { get; set; }

        public decimal Price { get; set; }  // Ensure lowercase "decimal" for consistency
        public int Quantity { get; set; }

        [ForeignKey("ProductId")]
        public ProductModel Product { get; set; }
    }
}
