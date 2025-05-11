﻿using System.Net.Mail;
using System.Net;

namespace Shopping_Laptop.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("nguyenducan1526@gmail.com", "zrkdyrkdjuzfkime")
            };

            return client.SendMailAsync(
                new MailMessage(from: "nguyenducan1526@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}