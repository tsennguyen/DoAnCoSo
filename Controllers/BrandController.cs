﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;

namespace Shopping_Laptop.Controllers
{
    public class BrandController : Controller
    {
        private readonly DataContext _dataContext;
        public BrandController(DataContext context)
        {
            _dataContext = context;
        }
        public async Task<IActionResult> Index(string Slug = "")
        {
            BrandModel brand = _dataContext.Brands.Where(c => c.Slug == Slug).FirstOrDefault();

            if (brand == null) return View(new List<ProductModel>());
          
            var productsByBrand = _dataContext.Products.Where(p => p.BrandId == brand.Id);


            return View(await productsByBrand.OrderByDescending(p => p.Id).ToListAsync());
        }
    }
}
