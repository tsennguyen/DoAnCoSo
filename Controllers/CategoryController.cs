using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;

namespace Shopping_Laptop.Controllers
{
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;

        public CategoryController(DataContext context)
        {
            _dataContext = context;
        }

        // Hiển thị danh sách sản phẩm theo danh mục (Slug)
        public async Task<IActionResult> Index(string Slug = "", string sort_by = "", string startprice = "", string endprice = "")
        {
            //Lấy tất cả sản phẩm 


            // Tìm danh mục theo slug
            CategoryModel category = await _dataContext.Categories.FirstOrDefaultAsync(c => c.Slug == Slug);


            if (category == null)
            {
                // Nếu không tìm thấy, trả về view trống
                ViewBag.Message = "Danh mục không tồn tại hoặc đã bị xóa.";
                return RedirectToAction("Index");
            }

            ViewBag.Slug = Slug;
            //lay tat ca san pham
            IQueryable<ProductModel> productsByCategory = _dataContext.Products.Where(p => p.CategoryId == category.Id);
            var count = await productsByCategory.CountAsync();

            if (count > 0) 
            {
                // Apply sorting based on sort_by parameter
                if (sort_by == "price_increase")
                {
                    productsByCategory = productsByCategory.OrderBy(p => p.Price);
                }
                else if (sort_by == "price_decrease")
                {
                    productsByCategory = productsByCategory.OrderByDescending(p => p.Price);
                }
                else if (sort_by == "price_newest")
                {
                    productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                }
                else if (sort_by == "price_oldest")
                {
                    productsByCategory = productsByCategory.OrderBy(p => p.Id);
                }


                //loc gia
                else if (startprice != "" && endprice != "")
                {
                    decimal startPriceValue;
                    decimal endPriceValue;

                    if (decimal.TryParse(startprice, out startPriceValue) && decimal.TryParse(endprice, out endPriceValue))
                    {
                        productsByCategory = productsByCategory.Where(p => p.Price >= startPriceValue && p.Price <= endPriceValue);
                    }
                    else
                    {
                        productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                    }
                }
                else
                { 
                    productsByCategory = productsByCategory.OrderByDescending(p => p.Id);
                }
            }
            return View(await productsByCategory.ToListAsync());
        }
    }
}
