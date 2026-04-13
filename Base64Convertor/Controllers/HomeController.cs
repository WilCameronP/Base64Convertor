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
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text;
using System.Threading;

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
        public IActionResult Index([FromServices]IContentTypeProvider contentTypeProvider, [FromServices]IMemoryCache memoryCache, Base64 model)
        {
            if (ModelState.IsValid)
            {
                model.Text = model.Text.Replace('-', '+');
                model.Text = model.Text.Replace('_', '/');
                model.Text = model.Text.Replace('=', '/');
                model.Text = model.Text.Replace(':', '/');
                model.Text = model.Text.Replace("\\", "");
                byte[] bytes = Convert.FromBase64String(model.Text);
                string fileExtension = model.FileType == "Other" ? model.OtherFileType : model.FileType;
                if (contentTypeProvider.TryGetContentType(fileExtension, out string mime))
                {
                    string fileName = Guid.NewGuid().ToString("N") + fileExtension;

                    memoryCache.Set(fileName, new FileCache { Contents = bytes, Mime = mime, Name = fileName }, TimeSpan.FromMinutes(11));

                    HttpContext.Response.Headers.Append("Content-Disposition", "inline; filename=" + fileName); // https://stackoverflow.com/questions/19411335/make-a-file-open-in-browser-instead-of-downloading-it
                    return File(bytes, mime);
                }
                throw new Exception($"File type {fileExtension} is not supported.");
            }
            return View();
        }

        public IActionResult File([FromServices]IContentTypeProvider contentTypeProvider, [FromServices]IMemoryCache memoryCache, string id)
        {
            //Zebra.Sdk.Comm.
            //Neodynamic.SDK.ZPLPrinter.ZPLPrinter
            if (!string.IsNullOrWhiteSpace(id) && memoryCache.TryGetValue<FileCache>(id, out var item))
            {
                HttpContext.Response.Headers.Append("Content-Disposition", "inline; filename=" + item.Name); // https://stackoverflow.com/questions/19411335/make-a-file-open-in-browser-instead-of-downloading-it
                return File(item.Contents, item.Mime);
            }
            return Redirect(nameof(Index));
        }
        #endregion

        #region File to Base64 String
        public IActionResult Reverse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Reverse(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
                throw new Exception("File is not selected.");
            if (file.Length > 50 * 1024 * 1024)
                throw new Exception("File size can not more than 50M.");
            Stream stream = file.OpenReadStream();
            
            var buffer = new byte[file.Length];
            await stream.ReadExactlyAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
            string base64 = Convert.ToBase64String(buffer);
            return Content(base64);
        }
        #endregion

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //private void Larray()
        //{
        //    byte[] zpl = Encoding.UTF8.GetBytes("^xa^cfa,50^fo100,100^fdHello World^fs^xz");
            
        //    // adjust print density (8dpmm), label width (4 inches), label height (6 inches), and label index (0) as necessary
        //    var request = (HttpWebRequest)WebRequest.Create("http://api.labelary.com/v1/printers/8dpmm/labels/4x6/0/");
        //    request.Method = "POST";
        //    request.Accept = "application/pdf"; // omit this line to get PNG images back
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ContentLength = zpl.Length;

        //    var requestStream = request.GetRequestStream();
        //    requestStream.Write(zpl, 0, zpl.Length);
        //    requestStream.Close();

        //    try
        //    {
        //        var response = (HttpWebResponse)request.GetResponse();
        //        var responseStream = response.GetResponseStream();
        //        var fileStream = System.IO.File.Create("label.pdf"); // change file name for PNG images
        //        responseStream.CopyTo(fileStream);
        //        responseStream.Close();
        //        fileStream.Close();
        //    }
        //    catch (WebException e)
        //    {
        //        Console.WriteLine("Error: {0}", e.Status);
        //    }
        //}
    }
}
