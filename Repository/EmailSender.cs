using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Shopping_Laptop.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message, bool isHtml = false)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, // Bật SSL để bảo mật kết nối
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(
                    "thanh.nv@greenabc.pet", // Email gửi
                    "sgzpodqlmyoadsli" // Mật khẩu ứng dụng (không nên lưu mật khẩu trực tiếp trong mã nguồn)
                )
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("thanh.nv@greenabc.pet"),
                Subject = subject,
                Body = message,
                IsBodyHtml = isHtml // Nếu isHtml = true thì gửi email dạng HTML
            };

            mailMessage.To.Add(email); // Thêm email người nhận

            return client.SendMailAsync(mailMessage); // Gửi email
        }
    }
}
