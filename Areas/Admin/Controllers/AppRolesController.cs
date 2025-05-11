using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Shopping_Laptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/AppRoles")]
    [Authorize(Roles= "Admin")]
    public class AppRolesController : Controller
    {

        private readonly RoleManager<IdentityRole> _roleManager;

        public AppRolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
