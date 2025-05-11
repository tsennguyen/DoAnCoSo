using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shopping_Laptop.Models;
using Shopping_Laptop.Models.ViewModels;
using Shopping_Laptop.Repository;
using Shopping_Laptop.Services.Momo;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace Shopping_Laptop.Controllers
{
    public class CheckoutController : Controller
    {

        private readonly DataContext _dataContext;
        private readonly IEmailSender _emailSender;
        private IMomoService _momoService;
        //private readonly IVnPayService _vnPayService;
        private static readonly HttpClient client = new HttpClient();
        public CheckoutController(IEmailSender emailSender, DataContext context, IMomoService momoService)
        {
            _dataContext = context;
            _emailSender = emailSender;
            _momoService = momoService;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Checkout(string OrderId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            //Nhận Coupon code từ cookie
            var coupon_code = Request.Cookies["CouponTitle"];


            var ordercode = Guid.NewGuid().ToString();
            var orderItem = new OrderModel();
            orderItem.OrderCode = ordercode;
            orderItem.CouponCode = coupon_code;
            orderItem.UserName = userEmail;
            orderItem.Status = 1;
            orderItem.CreatedDate = DateTime.Now;
            // Retrieve shipping price from cookie
            var shippingPriceCookie = Request.Cookies["ShippingPrice"];
            decimal shippingPrice = 0;

            if (shippingPriceCookie != null)
            {
                var shippingPriceJson = shippingPriceCookie;
                shippingPrice = JsonConvert.DeserializeObject<decimal>(shippingPriceJson);
            }
            else
            {
                shippingPrice = 0;
            }
            if (OrderId != null)

            {
                orderItem.PaymentMethod = OrderId;

            }
            else
            {
                orderItem.PaymentMethod = "COD";
            }
            orderItem.ShippingCost = shippingPrice;
            //Nhận coupon code
            var CouponCode = Request.Cookies["CouponTitle"];
            //orderItem.CouponCode = CouponCode;
            _dataContext.Add(orderItem);
            _dataContext.SaveChanges();
            //tạo order detail
            List<CartItemModel> cartItems = HttpContext.Session.GetJson<List<CartItemModel>>("Cart") ?? new List<CartItemModel>();
            foreach (var cart in cartItems)
            {
                var orderdetail = new OrderDetails();
                orderdetail.UserName = userEmail;
                orderdetail.OrderCode = ordercode;
                orderdetail.ProductId = (int)cart.ProductId;
                orderdetail.Price = cart.Price;
                orderdetail.Quantity = cart.Quantity;
                //update product quantity
                var product = await _dataContext.Products.Where(p => p.Id == cart.ProductId).FirstAsync();
                product.Quantity -= cart.Quantity;
                //product.SoldOut += cart.Quantity;
                _dataContext.Update(product);
                _dataContext.Add(orderdetail);
                _dataContext.SaveChanges();

            }
            HttpContext.Session.Remove("Cart");
            //Send mail order when success
            var receiver = userEmail;
            var subject = "Đặt hàng thành công";
            //var message = "Đặt hàng thành công, trải nghiệm dịch vụ nhé.";

            //await _emailSender.SendEmailAsync(receiver, subject, message);

            TempData["success"] = "Đơn hàng đã được tạo,vui lòng chờ duyệt đơn hàng nhé.";
            return RedirectToAction("History", "Account");

            return View();
        }
        [HttpGet]
        public async Task<IActionResult> PaymentCallBack(MomoInfoModel model)
        {
            var response = _momoService.PaymentExecuteAsync(HttpContext.Request.Query);
            var requestQuery = HttpContext.Request.Query;
            if (requestQuery["resultCode"] != 0) // giao dịch không thành công
            {
                var newMomoInsert = new MomoInfoModel
                {
                    OrderId = requestQuery["orderId"],
                    FullName = User.FindFirstValue(ClaimTypes.Email),
                    Amount = decimal.Parse(requestQuery["Amount"]),
                    OrderInfo = requestQuery["orderInfo"],
                    DatePaid = DateTime.Now
                };

                _dataContext.Add(newMomoInsert);
                await _dataContext.SaveChangesAsync();
                await Checkout(requestQuery["orderId"]);
            }
            else
            {
                TempData["success"] = "Giao dịch Momo không thành công.";
                return RedirectToAction("Index", "Cart");
            }

            return View(response);
        }
    }

}