using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Base64Convertor.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Base64Convertor.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(Base64 model)
        {
            if (ModelState.IsValid)
            {
                byte[] bytes = Convert.FromBase64String(model.Text);
                var stream = new MemoryStream(bytes);
                string mime = model.MIMEType == "Other" ? model.OtherMIMEType : model.MIMEType;
                return new FileStreamResult(stream, mime);
            }

            return View();
        }

        public IActionResult Reverse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reverse(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File not selected.");
            if (file.Length > 50 * 1024 * 1024)
                throw new Exception("File size can not more than 50M.");
            Stream stream = file.OpenReadStream();
            byte[] bytes = new byte[file.Length];
            await stream.ReadAsync(bytes, 0, (int)file.Length);
            string base64 = Convert.ToBase64String(bytes);
            return Content(base64);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
