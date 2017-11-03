using System;
using System.Security.Cryptography;
using MainSite.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;

namespace MainSite.Controllers
{
    //OWASP #6 Sensitive data exposure

    public class CiphersController : Controller
    {
        private readonly IDataProtector _protector;

        public CiphersController(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector(GetType().FullName);
        }

        public IActionResult Index()
        {
            return View(new CiphersViewModel());
        }

        [HttpPost]
        public IActionResult Encrypt(CiphersViewModel model)
        {
            var encrypted = _protector.Protect(model.PlainTextInput);
            model.EncryptedOutput = encrypted;
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Decrypt(CiphersViewModel model)
        {
            var decrypted = _protector.Unprotect(model.EncryptedInput);
            model.DecryptedOutput = decrypted;
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult ComputeHash(CiphersViewModel model)
        {
            //https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/password-hashing
            var saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create()) { rng.GetBytes(saltBytes); }
            var hashedBytes = KeyDerivation.Pbkdf2(model.PlainTextPasswordInput, saltBytes, KeyDerivationPrf.HMACSHA512, 10000, 32);
            var saltedHash = Convert.ToBase64String(saltBytes) + ":" + Convert.ToBase64String(hashedBytes);
            model.HashedPasswordOutput = saltedHash;
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult VerifyHash(CiphersViewModel model)
        {
            var parts = model.HashedPasswordOutput.Split(':');
            var salt = Convert.FromBase64String(parts[0]);
            var hashedBytes = KeyDerivation.Pbkdf2(model.VerifyPasswordInput, salt, KeyDerivationPrf.HMACSHA512, 10000, 32);
            model.VerificationResult = parts[1].Equals(Convert.ToBase64String(hashedBytes));
            return View("Index", model);
        }

    }
}