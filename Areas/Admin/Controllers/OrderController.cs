using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;

namespace Shopping_Laptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Employee,Company")]
    public class OrderController : Controller
    {
        private readonly DataContext _dataContext;

        public OrderController(DataContext context)
        {
            _dataContext = context;
        }

        // GET: Danh sách đơn hàng (có phân trang)
        public async Task<IActionResult> Index(int pg = 1)
        {
            // Query orders and include CouponCode directly (without Include)
            var allOrders = await _dataContext.Orders
                .OrderByDescending(o => o.CreatedDate)
                .Select(o => new OrderModel
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    ShippingCost = o.ShippingCost,
                    CouponCode = o.CouponCode,
                    UserName = o.UserName,
                    CreatedDate = o.CreatedDate,
                    Status = o.Status
                })
                .ToListAsync();

            const int pageSize = 10;
            if (pg < 1) pg = 1;

            int recsCount = allOrders.Count();
            var pager = new Paginate(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;

            var data = allOrders.Skip(recSkip).Take(pager.PageSize).ToList();
            ViewBag.Pager = pager;

            // Pass the user email to the view
            ViewBag.UserEmail = User.Identity.Name;  // Assuming this is the logged-in user's email

            return View(data);
        }



        // GET: Xem chi tiết đơn hàng
        public async Task<IActionResult> ViewOrder(string ordercode)
        {
            if (string.IsNullOrEmpty(ordercode))
                return NotFound();

            var orderDetails = await _dataContext.OrderDetails
                .Include(od => od.Product)
                .Where(od => od.OrderCode == ordercode)
                .ToListAsync();

            if (!orderDetails.Any())
                return NotFound();

            ViewBag.OrderCode = ordercode;
            ViewBag.SelectedStatus = _dataContext.Orders
                                        .Where(x => x.OrderCode == ordercode)
                                        .Select(x => x.Status)
                                        .FirstOrDefault();
            var ShippingCost = _dataContext.Orders.Where(o => o.OrderCode == ordercode).First();
            ViewBag.ShippingCost = ShippingCost.ShippingCost;

            return View(orderDetails);
        }

        // POST: Cập nhật trạng thái đơn hàng
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(string ordercode, int status)
        {
            if (string.IsNullOrEmpty(ordercode))
                return BadRequest();
            var order = await _dataContext.Orders.FirstOrDefaultAsync(o => o.OrderCode == ordercode);
            if (order == null)
                return NotFound();

            order.Status = status;
            await _dataContext.SaveChangesAsync();

            TempData["success"] = "Cập nhật trạng thái đơn hàng thành công";
            return RedirectToAction("ViewOrder", new { ordercode = ordercode });
        }

        [HttpGet]
        [Route("PaymentMomoInfo")]
        public async Task<IActionResult> PaymentMomoInfo(string orderId)
        {
            var momoInfo = await _dataContext.MomoInfos.FirstOrDefaultAsync(m => m.OrderId == orderId);

            if (momoInfo == null)
            {
                return NotFound();
            }
            return View(momoInfo);
        }
    }
}