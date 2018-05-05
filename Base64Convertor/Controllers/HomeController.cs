using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Base64Convertor.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace Base64Convertor.Controllers
{
    public class HomeController : Controller
    {
        #region Base64 String to File
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index([FromServices]IContentTypeProvider contentTypeProvider, Base64 model)
        {
            if (ModelState.IsValid)
            {
                byte[] bytes = Convert.FromBase64String(model.Text);
                string fileExtension = model.FileType == "Other" ? model.OtherFileType : model.FileType;
                if (contentTypeProvider.TryGetContentType(fileExtension, out string mime))
                {
                    string fileName = Guid.NewGuid().ToString("N") + fileExtension;                    
                    HttpContext.Response.Headers.Add("Content-Disposition", "inline; filename=" + fileName); // https://stackoverflow.com/questions/19411335/make-a-file-open-in-browser-instead-of-downloading-it
                    return File(bytes, mime);
                }
                throw new Exception($"File type {fileExtension} is not supported.");
            }
            return View();
        }
        #endregion

        #region File to Base64 String
        public IActionResult Reverse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reverse(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is not selected.");
            if (file.Length > 50 * 1024 * 1024)
                throw new Exception("File size can not more than 50M.");
            Stream stream = file.OpenReadStream();
            byte[] bytes = new byte[file.Length];
            await stream.ReadAsync(bytes, 0, (int)file.Length);
            string base64 = Convert.ToBase64String(bytes);
            return Content(base64);
        }
        #endregion

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
