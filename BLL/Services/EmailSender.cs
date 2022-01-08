using BLL.Interfaces;
using DAL.EF.Entities;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace BLL.Services
{
    public class EmailSender : IEmailSender
    {
        //Отправка на почту для востановления пароля
        public void SendEmail(User user)
        {
            var builder = new ConfigurationBuilder()
                              .AddJsonFile(@"C:\Users\dimas\source\repos\MuseumLouvre\BLL\appsettings.json");

            var config = builder.Build();

            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress("email@gmail.com");
                mail.To.Add(user.Email);
                mail.Subject = "Востановление пароля";
                mail.Body = "Привет: " + user.Name + " \tТвой пароль: " + user.Password;

                using (SmtpClient smtp = new SmtpClient(config["Smtp:Host"], int.Parse(config["Smtp:Port"])))
                {
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(config["Smtp:Username"], config["Smtp:Password"]);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }
        }
    }
}
