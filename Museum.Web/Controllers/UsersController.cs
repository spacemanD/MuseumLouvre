using BLL.Interfaces;
using DAL.EF.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Museum.Web.Controllers
{
    public class UsersController : Controller
    {

        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "Exhibit");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await Task.Run(() => _service.GetAllListAsync().Result.ToList().FirstOrDefault(x => x.Id == id));
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        public async Task<IActionResult> EditByLogin()
        {
            var us = await _service.FindByLogin(User.Identity.Name);
            await Task.Run(() => _service.UpdateAsync(us));
            return View("Edit", us);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Email,Login,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await Task.Run(() => _service.UpdateAsync(user));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Exhibit");
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await Task.Run(() => _service.GetAllListAsync().Result.ToList().FirstOrDefault(x => x.Id == id));
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await Task.Run(() => _service.GetAllListAsync().Result.ToList().FirstOrDefault(x => x.Id == id));
            await Task.Run(() => _service.DeleteAsync(user));
            Thread.Sleep(1000);
            return RedirectToAction("Login","Account");
        }

        private bool UserExists(int id)
        {
            return _service.GetAllListAsync().Result.ToList().Any(x => x.Id == id);
        }
    }
}
