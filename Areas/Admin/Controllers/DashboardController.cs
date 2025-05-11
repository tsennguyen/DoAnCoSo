using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopping_Laptop.Models;
using Shopping_Laptop.Repository;

namespace Shopping_Laptop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/Dashboard")]
    [Authorize(Roles = "Admin, Company")]
    public class DashboardController : Controller
    {
        private const int v = 2024;
        private readonly DataContext _dataContext;
        public DashboardController(DataContext context)
        {
            _dataContext = context;
        }
        public IActionResult Index()
        {
            var count_product = _dataContext.Products.Count();
            var count_order = _dataContext.Orders.Count();
            var count_category = _dataContext.Categories.Count();
            var count_user = _dataContext.Users.Count();
            ViewBag.CountProduct = count_product;
            ViewBag.CountOrder = count_order;
            ViewBag.CountCategory = count_category;
            ViewBag.CountUser = count_user;
            return View();
        }
        [HttpPost]
        [Route("SubmitFilterDate")]
        public IActionResult SubmitFilterDate(string filterdate)
        {
            if (!DateTime.TryParse(filterdate, out DateTime selectedDate))
            {
                return BadRequest("Invalid date format.");
            }

            // Chuyển ngày thành chỉ ngày (bỏ giờ để so sánh chính xác)
            var dateOnly = selectedDate.Date;

            var chartData = _dataContext.Orders
                .Where(o => o.CreatedDate.Date == dateOnly)
                .Join(_dataContext.OrderDetails,
                    o => o.OrderCode,
                    od => od.OrderCode,
                    (o, od) => new
                    {
                        Date = o.CreatedDate.Date,
                        Revenue = od.Quantity * od.Price
                    })
                .GroupBy(x => x.Date)
                .Select(group => new StatisticalModel
                {
                    date = group.Key,
                    revenue = group.Sum(x => x.Revenue),
                    orders = group.Count()
                })
                .ToList();

            return Json(chartData);
        }

        [HttpPost]
        [Route("SelectFilterDate")]
        public IActionResult SelectFilterDate(string filterdate)
        {
            var chartData = new List<StatisticalModel>();
            // Initialize as empty list
            var today = DateTime.Today;
            var month = new DateTime(today.Year, today.Month, 1);
            var first = month.AddMonths(-1);
            var last = month.AddDays(-1);


            if (filterdate == "last_month")
            {
                chartData = _dataContext.Orders
               .Where(o => o.CreatedDate > first && o.CreatedDate < today)

               .Join(_dataContext.OrderDetails,
                 o => o.OrderCode,
                 od => od.OrderCode,
                 (o, od) => new StatisticalModel
                 {
                     date = o.CreatedDate,
                     revenue = od.Quantity * od.Price, // Calculate revenue based on order details
                     orders = 1 // Assuming each order detail represents one order
                 })
                 .GroupBy(s => s.date)
                 .Select(group => new StatisticalModel
                 {
                     date = group.Key,
                     revenue = group.Sum(s => s.revenue),
                     orders = group.Count()
                 })
                 .ToList();
            }


            return Json(chartData);
        }
        [HttpPost]
        [Route("GetChartData")]
        public IActionResult GetChartData()
        {

            var chartData = _dataContext.Orders
          .Join(_dataContext.OrderDetails,
              o => o.OrderCode,
              od => od.OrderCode,
              (o, od) => new StatisticalModel
              {
                  date = o.CreatedDate,
                  revenue = od.Quantity * od.Price, // Calculate revenue based on order details
                  orders = 1 // Assuming each order detail represents one order
              })
          .GroupBy(s => s.date)
          .Select(group => new StatisticalModel
          {
              date = group.Key,
              revenue = group.Sum(s => s.revenue),
              orders = group.Count()
          })
          .ToList();

            return Json(chartData);
        }

    }
}