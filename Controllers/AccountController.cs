using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google; // Ensure this is added
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Shopping_Laptop.Models;
using Shopping_Laptop.Models.ViewModels;
using System.Threading.Tasks;
using Shopping_Laptop.Repository;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace Shopping_Laptop.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userManage;
        private SignInManager<AppUserModel> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly DataContext _dataContext;
        public AccountController(IEmailSender emailSender, UserManager<AppUserModel> userManage,
            SignInManager<AppUserModel> signInManager, DataContext context)
        {
            _userManage = userManage;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _dataContext = context;

        }
        public IActionResult Login(string returnUrl)
        {
            return View();
        }

    /*    public async Task<IActionResult> UpdateAccount(string returnUrl)
        {
            if (!User.Identity?.IsAuthenticated ?? false)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login", "Account");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var user = await _userManage.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                TempData["error"] = "User not found";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                // Set the existing values in the ViewBag or Model for pre-population
                ViewBag.UserEmail = userEmail;
                ViewBag.UserName = user.UserName;
                ViewBag.UserId = user.Id;
            }

            return View(user); // Return the view with the user data for editing
        }*/

     /*   [HttpPost]
        public async Task<IActionResult> UpdateAccount(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManage.Users.FirstOrDefaultAsync(u => u.Id == model.Id);
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.Password = model.Password; // Ensure you're hashing the password properly before saving

                    _userManage.UpdateAsync(user);
                

                    TempData["success"] = "Account updated successfully!";
                    return RedirectToAction("Profile", "Account"); // Redirect to a user profile or dashboard
                }
                else
                {
                    TempData["error"] = "User not found";
                    return RedirectToAction("Login", "Account");
                }
            }

            return View(model);
        }*/


        [HttpPost]
        public async Task<IActionResult> SendMailForgotPass(AppUserModel user)
        {
            var checkMail = await _userManage.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (checkMail == null)
            {
                TempData["error"] = "Email not found";
                return RedirectToAction("ForgotPass", "Account");
            }
            else
            {
                string token = Guid.NewGuid().ToString();
                //update token to user
                checkMail.Token = token;
                _dataContext.Update(checkMail);
                await _dataContext.SaveChangesAsync();
                var receiver = checkMail.Email;
                var subject = "Change password for user " + checkMail.Email;
                var message = "Click on link to change password " +
                    "<a href='" + $"{Request.Scheme}://{Request.Host}/Account/NewPass?email=" + checkMail.Email + "&token=" + token + "'>";

                await _emailSender.SendEmailAsync(receiver, subject, message);
            }


            TempData["success"] = "An email has been sent to your registered email address with password reset instructions.";
            return RedirectToAction("ForgotPass", "Account");
        }
        public IActionResult ForgotPass()
        {
            return View();
        }
        public async Task<IActionResult> NewPass(AppUserModel user, string token)
        {
            var checkuser = await _userManage.Users
                .Where(u => u.Email == user.Email)
                .Where(u => u.Token == user.Token).FirstOrDefaultAsync();

            if (checkuser != null)
            {
                ViewBag.Email = checkuser.Email;
                ViewBag.Token = token;
            }
            else
            {
                TempData["error"] = "Email not found or token is not right";
                return RedirectToAction("ForgotPass", "Account");
            }
            return View();
        }
        public async Task<IActionResult> UpdateNewPassword(AppUserModel user, string token)
        {
            var checkuser = await _userManage.Users
                .Where(u => u.Email == user.Email)
                .Where(u => u.Token == user.Token).FirstOrDefaultAsync();

            if (checkuser != null)
            {
                //update user with new password and token
                string newtoken = Guid.NewGuid().ToString();
                // Hash the new password
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var passwordHash = passwordHasher.HashPassword(checkuser, user.PasswordHash);

                checkuser.PasswordHash = passwordHash;
                checkuser.Token = newtoken;

                await _userManage.UpdateAsync(checkuser);
                TempData["success"] = "Password updated successfully.";
                return RedirectToAction("Login", "Account");
            }
            else
            {
                TempData["error"] = "Email not found or token is not right";
                return RedirectToAction("ForgotPass", "Account");
            }
            return View();
        }
        public async Task<IActionResult> History()
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login", "Account"); // Replace "Account" with your controller name
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var Orders = await _dataContext.Orders
                .Where(od => od.UserName == userEmail).OrderByDescending(od => od.Id).ToListAsync();
            ViewBag.UserEmail = userEmail;
            return View(Orders);
        }

        public async Task<IActionResult> CancelOrder(string ordercode)
        {
            if ((bool)!User.Identity?.IsAuthenticated)
            {
                // User is not logged in, redirect to login
                return RedirectToAction("Login", "Account");
            }
            try
            {
                var order = await _dataContext.Orders.Where(o => o.OrderCode == ordercode).FirstAsync();
                order.Status = 3;
                _dataContext.Update(order);
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                return BadRequest("An error occurred while canceling the order.");
            }


            return RedirectToAction("History", "Account");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    var user = await _userManage.FindByNameAsync(loginVM.UserName);
                    var roles = await _userManage.GetRolesAsync(user);


                    if (roles.Contains("Admin"))
                    {
                        return RedirectToAction("Index", "Admin"); // hoặc tên controller admin
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home"); // người dùng thường
                    }
                    //TempData["success"] = "Đăng nhập thành công";
                    //var receiver = "demologin979@gmail.com";
                    //var subject = "Đăng nhập trên thiết bị thành công.";
                    var message = @"
<html>
<head>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            padding: 20px;
        }
        .container {
            background-color: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        h2 {
            color: #333333;
            font-size: 24px;
        }
        .content {
            margin-top: 20px;
            font-size: 16px;
            line-height: 1.5;
            color: #666;
        }
        .footer {
            margin-top: 30px;
            color: #888;
            font-size: 14px;
            text-align: center;
        }
        .button {
            background-color: #007bff;
            color: white;
            padding: 12px 30px;
            border-radius: 5px;
            text-decoration: none;
            font-size: 16px;
            display: inline-block;
            margin-top: 20px;
        }
        .button:hover {
            background-color: #0056b3;
        }
        .signature {
            margin-top: 30px;
            font-size: 16px;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <div class='container'>
        <h2>Kính gửi {userEmail},</h2>
        <p class='content'>Chúng tôi rất vui mừng thông báo rằng bạn đã đăng nhập thành công vào tài khoản của mình trên website của chúng tôi.</p>

        <p class='content'>Chúc mừng bạn đã gia nhập cộng đồng mua sắm trực tuyến của chúng tôi. Tại TCon, bạn có thể dễ dàng khám phá và lựa chọn các sản phẩm công nghệ hàng đầu như điện thoại di động, phụ kiện điện thoại, máy tính xách tay, và nhiều thiết bị điện tử cao cấp khác.</p>

        <p class='content'>Chúng tôi cam kết mang đến cho bạn những trải nghiệm mua sắm tiện lợi, an toàn, và nhanh chóng. Với đội ngũ hỗ trợ khách hàng tận tình, bạn sẽ luôn nhận được sự hỗ trợ kịp thời mỗi khi cần.</p>

        <p class='content'>Nếu bạn cần bất kỳ sự trợ giúp nào hoặc có thắc mắc về sản phẩm, vui lòng liên hệ với chúng tôi qua email hoặc số điện thoại hỗ trợ khách hàng.</p>

        <p class='content'>Cảm ơn bạn đã tin tưởng và lựa chọn TCon!</p>

        <a href='https://www.tcon.com' class='button'>Khám Phá Ngay</a>

        <div class='footer'>
            <p>Trân trọng,<br>Đội ngũ TCon – Cửa hàng điện thoại và phụ kiện công nghệ</p>
        </div>
    </div>
</body>
</html>
";

                    //await _emailSender.SendEmailAsync(receiver, subject, message);
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Sai tài khoản hặc mật khẩu");
            }
            return View(loginVM);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel { UserName = user.UserName, Email = user.Email };
                IdentityResult result = await _userManage.CreateAsync(newUser, user.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "Tạo thành viên thành công";
                    return Redirect("/account/login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }


        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            await HttpContext.SignOutAsync();
            return Redirect(returnUrl);
        }



        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string emailName = email.Split('@')[0];
            var existingUser = await _userManage.FindByEmailAsync(email);

            if (existingUser == null)
            {
                var passwordHasher = new PasswordHasher<AppUserModel>();
                var hashedPassword = passwordHasher.HashPassword(null, "123456789");
                var newUser = new AppUserModel { UserName = emailName, Email = email };
                newUser.PasswordHash = hashedPassword;

                var createUserResult = await _userManage.CreateAsync(newUser);
                if (!createUserResult.Succeeded)
                {
                    TempData["error"] = "Đăng ký tài khoản thất bại. Vui lòng thử lại sau.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    await _signInManager.SignInAsync(newUser, isPersistent: false);
                    TempData["success"] = "Đăng ký tài khoản thành công.";
                }
            }
            else
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
            }

            // Gửi email khi đăng nhập thành công
            var subject = "Thông báo đăng nhập thành công";
            var message = $@"
<html>
<head>
    <style>
        body {{
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            padding: 20px;
        }}
        .container {{
            background-color: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }}
        h2 {{
            color: #333333;
            font-size: 24px;
        }}
        .content {{
            margin-top: 20px;
            font-size: 16px;
            line-height: 1.5;
            color: #666;
        }}
        .footer {{
            margin-top: 30px;
            color: #888;
            font-size: 14px;
            text-align: center;
        }}
        .button {{
            background-color: #007bff;
            color: white;
            padding: 12px 30px;
            border-radius: 5px;
            text-decoration: none;
            font-size: 16px;
            display: inline-block;
            margin-top: 20px;
        }}
        .button:hover {{
            background-color: #0056b3;
        }}
        .signature {{
            margin-top: 30px;
            font-size: 16px;
            font-weight: bold;
        }}
    </style>
</head>
<body>
    <div class='container'>
        <h2>Kính gửi {email},</h2>
        <p class='content'>Chúng tôi rất vui mừng thông báo rằng bạn đã đăng nhập thành công vào tài khoản của mình trên website của chúng tôi.</p>
        <p class='content'>Chúc mừng bạn đã gia nhập cộng đồng mua sắm trực tuyến của chúng tôi. Tại TCon, bạn có thể dễ dàng khám phá và lựa chọn các sản phẩm công nghệ hàng đầu như điện thoại di động, phụ kiện điện thoại, máy tính xách tay, và nhiều thiết bị điện tử cao cấp khác.</p>
        <p class='content'>Chúng tôi cam kết mang đến cho bạn những trải nghiệm mua sắm tiện lợi, an toàn, và nhanh chóng. Với đội ngũ hỗ trợ khách hàng tận tình, bạn sẽ luôn nhận được sự hỗ trợ kịp thời mỗi khi cần.</p>
        <p class='content'>Nếu bạn cần bất kỳ sự trợ giúp nào hoặc có thắc mắc về sản phẩm, vui lòng liên hệ với chúng tôi qua email hoặc số điện thoại hỗ trợ khách hàng.</p>
        <p class='content'>Cảm ơn bạn đã tin tưởng và lựa chọn TCon!</p>
        <a href='https://www.tcon.com' class='button'>Khám Phá Ngay</a>
        <div class='footer'>
            <p>Trân trọng,<br>Đội ngũ TCon – Cửa hàng điện thoại và phụ kiện công nghệ</p>
        </div>
    </div>
</body>
</html>
";

            await _emailSender.SendEmailAsync(email, subject, message);

            TempData["Success"] = "Đăng Nhập Thành Công";
            return RedirectToAction("Index", "Home");
        }


        [Authorize(Roles = "Admin,Company,Employee")]
        public IActionResult IndexAdmin(string returnUrl = "/Admin")
        {
            return Redirect(returnUrl);
        }

        public async Task LoginByGoogle()
        {
            // Use Google authentication scheme for challenge
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("GoogleResponse")
                });
        }
    }
}
/*var message = @"
<html>
<head>
    <style>
        body {
            font-family: Arial, sans-serif;
            background-color: #f4f4f4;
            padding: 20px;
        }
        .container {
            background-color: #ffffff;
            padding: 30px;
            border-radius: 8px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
        }
        h2 {
            color: #333333;
            font-size: 24px;
        }
        .content {
            margin-top: 20px;
            font-size: 16px;
            line-height: 1.5;
            color: #666;
        }
        .footer {
            margin-top: 30px;
            color: #888;
            font-size: 14px;
            text-align: center;
        }
        .button {
            background-color: #007bff;
            color: white;
            padding: 12px 30px;
            border-radius: 5px;
            text-decoration: none;
            font-size: 16px;
            display: inline-block;
            margin-top: 20px;
        }
        .button:hover {
            background-color: #0056b3;
        }
        .signature {
            margin-top: 30px;
            font-size: 16px;
            font-weight: bold;
        }
    </style>
</head>
<body>
    <div class='container'>
        <h2>Kính gửi {userEmail},</h2>
        <p class='content'>Chúng tôi rất vui mừng thông báo rằng bạn đã đăng nhập thành công vào tài khoản của mình trên website của chúng tôi.</p>

        <p class='content'>Chúc mừng bạn đã gia nhập cộng đồng mua sắm trực tuyến của chúng tôi. Tại TCon, bạn có thể dễ dàng khám phá và lựa chọn các sản phẩm công nghệ hàng đầu như điện thoại di động, phụ kiện điện thoại, máy tính xách tay, và nhiều thiết bị điện tử cao cấp khác.</p>

        <p class='content'>Chúng tôi cam kết mang đến cho bạn những trải nghiệm mua sắm tiện lợi, an toàn, và nhanh chóng. Với đội ngũ hỗ trợ khách hàng tận tình, bạn sẽ luôn nhận được sự hỗ trợ kịp thời mỗi khi cần.</p>

        <p class='content'>Nếu bạn cần bất kỳ sự trợ giúp nào hoặc có thắc mắc về sản phẩm, vui lòng liên hệ với chúng tôi qua email hoặc số điện thoại hỗ trợ khách hàng.</p>

        <p class='content'>Cảm ơn bạn đã tin tưởng và lựa chọn TCon!</p>

        <a href='https://www.tcon.com' class='button'>Khám Phá Ngay</a>

        <div class='footer'>
            <p>Trân trọng,<br>Đội ngũ TCon – Cửa hàng điện thoại và phụ kiện công nghệ</p>
        </div>
    </div>
</body>
</html>
";*/