namespace Shopping_Laptop.Repository
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, bool isHtml = false); // Thêm tham số isHtml
    }
}
