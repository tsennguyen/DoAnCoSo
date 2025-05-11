using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;

namespace Shopping_Laptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee,Company")]
    public class ProductController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // Danh sách sản phẩm (có phân trang)
        public async Task<IActionResult> Index(int pg = 1)
        {
            const int pageSize = 10;
            if (pg < 1) pg = 1;

            var allProducts = _dataContext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .OrderByDescending(p => p.Id);

            int recsCount = await allProducts.CountAsync();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = await allProducts.Skip(recSkip).Take(pageSize).ToListAsync();
            ViewBag.Pager = pager;

            return View(data);
        }

        // GET: Tạo sản phẩm
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name");
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name");
            return View();
        }

        // POST: Tạo sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-").ToLower();
                var existingSlug = await _dataContext.Products.FirstOrDefaultAsync(p => p.Slug == product.Slug);

                if (existingSlug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã tồn tại.");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    Directory.CreateDirectory(uploadsDir);

                    string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(product.ImageUpload.FileName);
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(stream);
                    }

                    product.Image = imageName;
                }

                _dataContext.Add(product);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Thêm sản phẩm không thành công";
            return View(product);
        }

        // GET: Sửa sản phẩm
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            return View(product);
        }

        // POST: Sửa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductModel product)
        {
            if (id != product.Id)
                return BadRequest();

            var existingProduct = await _dataContext.Products.FindAsync(id);
            if (existingProduct == null)
                return NotFound();

            ViewBag.Categories = new SelectList(_dataContext.Categories, "Id", "Name", product.CategoryId);
            ViewBag.Brands = new SelectList(_dataContext.Brands, "Id", "Name", product.BrandId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.Replace(" ", "-").ToLower();
                var existingSlug = await _dataContext.Products
                    .FirstOrDefaultAsync(p => p.Slug == product.Slug && p.Id != product.Id);

                if (existingSlug != null)
                {
                    ModelState.AddModelError("", "Sản phẩm đã tồn tại.");
                    return View(product);
                }

                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                    Directory.CreateDirectory(uploadsDir);

                    string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(product.ImageUpload.FileName);
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await product.ImageUpload.CopyToAsync(stream);
                    }

                    // Xóa ảnh cũ nếu có
                    if (!string.IsNullOrEmpty(existingProduct.Image))
                    {
                        string oldPath = Path.Combine(uploadsDir, existingProduct.Image);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    existingProduct.Image = imageName;
                }

                // Cập nhật các trường khác
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Description = product.Description;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.BrandId = product.BrandId;
                existingProduct.Slug = product.Slug;

                _dataContext.Update(existingProduct);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Cập nhật sản phẩm không thành công";
            return View(product);
        }

        // Xóa sản phẩm
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dataContext.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            // Xóa ảnh
            if (!string.IsNullOrEmpty(product.Image) && !string.Equals(product.Image, "noimage.png"))
            {
                string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/products");
                string filePath = Path.Combine(uploadsDir, product.Image);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Xóa sản phẩm thành công";
            return RedirectToAction("Index");
        }
        //Thêm số lượng vào sản phẩm
        [Route("AddQuantity")]
        [HttpGet]
        public async Task<IActionResult> AddQuantity(int Id)
        {
            var productbyquantity = await _dataContext.ProductQuantities.Where(pq => pq.ProductId == Id).ToListAsync();
            ViewBag.ProductByQuantity = productbyquantity;
            ViewBag.Id = Id;
            return View();
        }
        [Route("StoreProductQuantity")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult StoreProductQuantity(ProductQuantityModel productQuantityModel)
        {
            var product = _dataContext.Products.Find(productQuantityModel.ProductId);
            if (product == null)
            {
                return NotFound();
            }
            product.Quantity += productQuantityModel.Quantity;

              
            productQuantityModel.Quantity = productQuantityModel.Quantity;
            productQuantityModel.ProductId = productQuantityModel.ProductId;
            productQuantityModel.DateCreated = DateTime.Now;

            _dataContext.Add(productQuantityModel);
            _dataContext.SaveChanges();
            TempData["success"] = "Thêm số lượng sản phẩm thành công";
            return RedirectToAction("AddQuantity","Product",new {Id = productQuantityModel.ProductId
        });
        }
    }
}
