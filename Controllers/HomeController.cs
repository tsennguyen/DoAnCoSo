using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;

namespace Shopping_Laptop.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUserModel> _userManager;


        public HomeController(ILogger<HomeController> logger, DataContext context, UserManager<AppUserModel> userManager)
        {
            _logger = logger;
            _dataContext = context;
            _userManager = userManager;
        }

        // Trang ch?: hi?n th? t?t c? s?n ph?m (có kèm theo danh m?c và th??ng hi?u)
        public IActionResult Index()
        {
            var products = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.Id)
                .ToList();

            return View(products);
        }

        // Trang chính sách b?o m?t (có th? c?p nh?t thêm n?i dung tùy ý)
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddWishlist(int Id, WishlistModel wishlistmodel)
        {
            var user = await _userManager.GetUserAsync(User);

            var wishlistProduct = new WishlistModel
            {  
               ProductId = Id,
               UserId = user.Id
            };
           
            _dataContext.Wishlist.Add(wishlistProduct);

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Đã thêm vào Yêu Thích" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã có lỗi xảy ra");
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddCompare(int Id, CompareModel compareModel)
        {
            var user = await _userManager.GetUserAsync(User);

            var compareProduct = new CompareModel
            {
                ProductId = Id,
                UserId = user.Id
            };

            _dataContext.Compare.Add(compareProduct);

            try
            {
                await _dataContext.SaveChangesAsync();
                return Ok(new { success = true, message = "Đã thêm vào  so sánh" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã có lỗi xảy ra");
            }
        }

        public async Task<IActionResult> Compare()
        {
            var compare_product = await (from c in _dataContext.Compare
                                         join p in _dataContext.Products on c.ProductId equals p.Id
                                         join u in _dataContext.Users on c.UserId equals u.Id
                                         select new { User = u, Product = p, Compare = c })
                                         .ToListAsync();

            return View(compare_product);
        }
        public async Task<IActionResult> DeleteCompare(int id)
        {
            CompareModel compare = await _dataContext.Compare.FindAsync(id);
           
            _dataContext.Compare.Remove(compare);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Compare", "Home");
        }
        public async Task<IActionResult> Wishlist()
        {
            var wishlist_product = await (from w in _dataContext.Wishlist
                                         join p in _dataContext.Products on w.ProductId equals p.Id
                                         join u in _dataContext.Users on w.UserId equals u.Id
                                         select new { User = u, Product = p, Wishlist = w })
                                         .ToListAsync();

            return View(wishlist_product);
        }
        public async Task<IActionResult> DeleteWishlist(int id)
        {
            WishlistModel wishlist = await _dataContext.Wishlist.FindAsync(id);

            _dataContext.Wishlist.Remove(wishlist);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa thành công";
            return RedirectToAction("Wishlist","Home");
        }




        // Trang liên h?
        public async Task<IActionResult> Contact()
        {
            var contact = await _dataContext.Contact.FirstAsync();
            return View(contact);
        }

        // X? lý l?i HTTP
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int statuscode)
        {
            if (statuscode == 404)
            {
                ViewBag.ErrorMessage = "Trang b?n yêu c?u không t?n t?i.";
                return View("NotFound");
            }
            else
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";
                return View(new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                });
            }
        }

       
    }
}
