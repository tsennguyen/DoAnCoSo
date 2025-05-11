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
    [Authorize(Roles = "Admin, Employee, Company")]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: Danh sách danh mục có phân trang
        [Route("Index")]
        public async Task<IActionResult> Index(int pg = 1)
        {
            var categoryList = _dataContext.Categories.ToList();
            const int pageSize = 10;
            if (pg < 1) pg = 1;

            int recsCount = categoryList.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = categoryList.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;
            return View(data);
        }

        // GET: Tạo danh mục
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tạo danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = GenerateSlug(category.Name);

                var existingCategory = await _dataContext.Categories
                    .FirstOrDefaultAsync(c => c.Slug == category.Slug);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại.");
                    return View(category);
                }

                _dataContext.Categories.Add(category);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm danh mục thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Thêm danh mục không thành công";
            return View(category);
        }

        // GET: Sửa danh mục
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Sửa danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryModel category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = GenerateSlug(category.Name);

                var existingCategory = await _dataContext.Categories
                    .FirstOrDefaultAsync(c => c.Slug == category.Slug && c.Id != category.Id);

                if (existingCategory != null)
                {
                    ModelState.AddModelError("", "Danh mục đã tồn tại.");
                    return View(category);
                }

                _dataContext.Categories.Update(category);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Cập nhật danh mục thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Cập nhật danh mục không thành công";
            return View(category);
        }

        // GET: Chi tiết danh mục
        public async Task<IActionResult> Details(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Xóa danh mục
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category == null)
            {
                TempData["error"] = "Không tìm thấy danh mục để xóa";
                return RedirectToAction("Index");
            }

            _dataContext.Categories.Remove(category);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa danh mục thành công";
            return RedirectToAction("Index");
        }

        // Tạo Slug chuẩn SEO
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
