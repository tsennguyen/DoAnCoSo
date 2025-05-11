using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Shopping_Laptop.Models
{
    public class AppUserModel : IdentityUser
    {
        public string Token { get; set; } // Add this line
        public string Email { get; set; }
        public string Occupation { get; set; }
       public string RoleId { get; set; }

        public string Password { get; set; }

        // Thêm thuộc tính SelectedRoles để sử dụng trong View
        public List<string> SelectedRoles { get; set; }
    }
}
