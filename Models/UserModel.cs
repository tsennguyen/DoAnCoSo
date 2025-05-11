using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace Shopping_Laptop.Models
{
    public class UserModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="Vui long nhap User")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Vui long nhap Email"),EmailAddress]
        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage ="Vui long nhap Password")]
        public string Password { get; set; }

        public string RoleId { get; set; }
    }
}
