using System.ComponentModel.DataAnnotations;

namespace Shopping_Laptop.Models.ViewModels
{
    public class LoginViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Vui long nhap User")]
        public string UserName { get; set; }
      
        [DataType(DataType.Password), Required(ErrorMessage = "Vui long nhap Password")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }
    }
}
