using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WFTileCounter.Models;

namespace WFTileCounter.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        //This is not currently set up to work. It is stored here for future development once out of testing phases
        // and I'll probably move it out of the HomeController.

        [HttpPost]
        public async Task<IActionResult> UploadFile(IEnumerable<IFormFile> fileList)
        {
            foreach (var file in fileList)
            {
                if (file == null || file.Length == 0)
                    return Content("file not selected");

                var path = Path.Combine(
                            System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Uploads",
                            file.FileName);


                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                else
                {
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            return View("Index");
        }


    }

   
    
 }

