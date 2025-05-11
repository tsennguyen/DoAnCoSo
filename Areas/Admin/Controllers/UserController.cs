using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shopping_Laptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataContext _dataContext;

        public UserController(UserManager<AppUserModel> userManager, RoleManager<IdentityRole> roleManager, DataContext dataContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataContext = dataContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Index(int pg = 1)
        {
            const int pageSize = 10;
            var users = await _userManager.Users.OrderByDescending(u => u.Id).ToListAsync();

            int recsCount = users.Count;
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var usersPaged = users.Skip(recSkip).Take(pageSize).ToList();

            var userViewModels = new List<AppUserModel>();
            foreach (var user in usersPaged)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.SelectedRoles = roles.ToList();
                userViewModels.Add(user);
            }

            ViewBag.Pager = pager;
            return View(userViewModels);
        }

        [HttpGet]
        [Route("Create")]
        public IActionResult Create()
        {
            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<IActionResult> Create(AppUserModel user, string password, List<string> selectedRoles)
        {
            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu.");
                return View(user);
            }

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    if (selectedRoles != null && selectedRoles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, selectedRoles);
                    }

                    TempData["success"] = "Tạo người dùng thành công";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            return View(user);
        }

        [HttpGet]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            ViewBag.UserRoles = await _userManager.GetRolesAsync(user);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Edit/{id}")]
        public async Task<IActionResult> Edit(string id, AppUserModel model, string password, List<string> SelectedRoles)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passResult = await _userManager.ResetPasswordAsync(user, token, password);

                if (!passResult.Succeeded)
                {
                    foreach (var error in passResult.Errors)
                        ModelState.AddModelError("", error.Description);

                    ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
                    ViewBag.UserRoles = await _userManager.GetRolesAsync(user);
                    return View(model);
                }
            }

            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (SelectedRoles != null && SelectedRoles.Any())
                await _userManager.AddToRolesAsync(user, SelectedRoles);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                    ModelState.AddModelError("", error.Description);

                ViewBag.Roles = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
                ViewBag.UserRoles = SelectedRoles;
                return View(model);
            }

            TempData["success"] = "Cập nhật người dùng thành công";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["error"] = "Xóa người dùng thất bại.";
                return RedirectToAction("Index");
            }

            TempData["success"] = "Xóa người dùng thành công";
            return RedirectToAction("Index");
        }

        // Xem đơn hàng của người dùng
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            var orderDetails = await _dataContext.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderCode == ordercode)
                .ToListAsync();

            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            ViewBag.SelectedStatus = order?.Status;

            return View(orderDetails);
        }
    }
}
