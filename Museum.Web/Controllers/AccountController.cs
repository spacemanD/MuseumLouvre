using BLL.Interfaces;
using DAL.EF.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using OnlineShop.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Museum.Web.Controllers
{
    public class AccountController : Controller
    {
        private IUserService service;
        private readonly IEmailSender sender;

        public AccountController(IUserService context, IEmailSender send)
        {
            service = context;
            sender = send;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await service.FindByLogin(model.Login);
                if (user != null)
                {
                    if (user.Password == model.Password)
                    {
                        await Authenticate(model.Login); // аутентификация

                        return RedirectToAction("Index", "Exhibit");
                    }
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await service.FindByLogin(model.Login);
                if (user == null)
                {
                    // добавляем пользователя в бд
                    await service.Create(new User { Name = model.Name, Email = model.Email, Login = model.Login, Password = model.Password });

                    await Authenticate(model.Login); // аутентификация

                    return RedirectToAction("Index", "Exhibit");
                }
                else
                    ModelState.AddModelError("", "User already exists");
            }
            return View(model);
        }
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([Bind("Login")] ForgotPassword model)
        {
            User user = await Task.Run(() => service.FindByLogin(model.Login).Result);
            sender.SendEmail(user);
            return RedirectToAction("Login");
        }

        private async Task Authenticate(string userName)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userName)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
