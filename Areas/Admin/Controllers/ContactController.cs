using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;

namespace Shopping_Laptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Contact")]
    [Authorize(Roles = "Admin")]
    public class ContactController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ContactController(DataContext context, IWebHostEnvironment webHostEnvironment)
        {
            _dataContext = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Contact/Index
        [Route("Index")]
        public IActionResult Index()
        {
            var contact = _dataContext.Contact.ToList();
            return View(contact);
        }

        // GET: Admin/Contact/Edit
        [Route("Edit")]
        [HttpGet]
        public IActionResult Edit()
        {
            var contact = _dataContext.Contact.FirstOrDefault();

            // Nếu không có dữ liệu nào thì khởi tạo mặc định
            if (contact == null)
            {
                var newContact = new ContactModel
                {
                    Name = "Tên công ty",
                    Email = "contact@example.com",
                    Phone = "0123456789",
                    Description = "Mô tả thông tin liên hệ",
                    Map = ""
                };

                _dataContext.Contact.Add(newContact);
                _dataContext.SaveChanges();

                contact = newContact;
            }

            return View(contact);
        }

        // POST: Admin/Contact/Edit
        [Route("Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactModel contact)
        {
            var existed_contact = _dataContext.Contact.FirstOrDefault();

            if (existed_contact == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh logo
                if (contact.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "media/logo");
                    Directory.CreateDirectory(uploadsDir);

                    string imageName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(contact.ImageUpload.FileName);
                    string filePath = Path.Combine(uploadsDir, imageName);

                    using (var fs = new FileStream(filePath, FileMode.Create))
                    {
                        await contact.ImageUpload.CopyToAsync(fs);
                    }

                    // Cập nhật tên file ảnh
                    existed_contact.LogoImg = imageName;
                }

                // Cập nhật các trường còn lại
                existed_contact.Name = contact.Name;
                existed_contact.Email = contact.Email;
                existed_contact.Description = contact.Description;
                existed_contact.Phone = contact.Phone;
                existed_contact.Map = contact.Map;

                _dataContext.Update(existed_contact);
                await _dataContext.SaveChangesAsync();

                TempData["success"] = "Cập nhật thông tin liên hệ thành công";
                return RedirectToAction("Index");
            }

            TempData["error"] = "Cập nhật không thành công";
            return View(contact);
        }
    }
}
