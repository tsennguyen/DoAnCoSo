using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;
using System.Text;
using System.Text.RegularExpressions;

namespace Shopping_Laptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin, Company")]
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;

        public BrandController(DataContext context)
        {
            _dataContext = context;
        }

        // Index - Hiển thị danh sách thương hiệu
        public async Task<IActionResult> Index(int pg = 1)
        {
            var brands = _dataContext.Brands.ToList();

            const int pageSize = 10;
            if (pg < 1) pg = 1;

            int recsCount = brands.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = brands.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(data);
        }

        // GET: Tạo thương hiệu
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tạo thương hiệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = GenerateSlug(brand.Name);

                var existingBrand = await _dataContext.Brands
                    .FirstOrDefaultAsync(b => b.Slug == brand.Slug);

                if (existingBrand != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại.");
                    return View(brand);
                }

                _dataContext.Brands.Add(brand);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm thương hiệu thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Thêm thương hiệu không thành công";
            return View(brand);
        }

        // GET: Chỉnh sửa thương hiệu
        public async Task<IActionResult> Edit(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        // POST: Chỉnh sửa thương hiệu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BrandModel brand)
        {
            if (ModelState.IsValid)
            {
                brand.Slug = GenerateSlug(brand.Name);

                var existingBrand = await _dataContext.Brands
                    .FirstOrDefaultAsync(b => b.Slug == brand.Slug && b.Id != brand.Id);

                if (existingBrand != null)
                {
                    ModelState.AddModelError("", "Thương hiệu đã tồn tại.");
                    return View(brand);
                }

                _dataContext.Brands.Update(brand);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Cập nhật thương hiệu thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Cập nhật thương hiệu không thành công";
            return View(brand);
        }

        // GET: Xóa thương hiệu
        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
            {
                TempData["error"] = "Không tìm thấy thương hiệu để xóa";
                return RedirectToAction("Index");
            }

            _dataContext.Brands.Remove(brand);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa thương hiệu thành công";
            return RedirectToAction("Index");
        }

        // GET: Chi tiết thương hiệu
        public async Task<IActionResult> Details(int id)
        {
            var brand = await _dataContext.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        // Helper: Generate Slug
        private string GenerateSlug(string phrase)
        {
            string str = RemoveAccent(phrase).ToLower();
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-").Trim();
            return str;
        }

        private string RemoveAccent(string text)
        {
            byte[] bytes = Encoding.GetEncoding("Cyrillic").GetBytes(text);
            return Encoding.ASCII.GetString(bytes);
        }
    }
}
