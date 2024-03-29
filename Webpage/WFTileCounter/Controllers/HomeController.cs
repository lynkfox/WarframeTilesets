﻿using System;
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
using WFTileCounter.BuisnessLogic;
using WFTileCounter.Models;

namespace WFTileCounter.Controllers
{
    public class HomeController : Controller
    {

        private readonly DatabaseContext _db; //database context shortcut
        private IHostingEnvironment _env;


        public HomeController(DatabaseContext context, IHostingEnvironment env)
        {
            _db = context;
            _env = env;
        }

        //testing page.
        public IActionResult Test()
        {
            return View("Test");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult TileCounter()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Code()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Tips()
        {
            return View();
        }

        public IActionResult UploadFiles()
        {
            return View("Upload");
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UploadFile(IEnumerable<IFormFile> fileList)
        {
            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.
            string userId = _gf.GetUserId().ToString();
            var webRoot = _env.WebRootPath;
            string directoryPath = Path.Combine(webRoot,"temp_uploads", userId);

            Directory.CreateDirectory(directoryPath);

            foreach (var file in fileList)
            {
                if (file == null || file.Length == 0)
                    return Content("file not selected");

               var imagePath = Path.Combine(directoryPath,
                            file.FileName);
                
                


                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        
                    }
                }
                else
                {
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                        
                    }

                }
                
            }


            ViewData["UserID"] = userId;
            return RedirectToAction("ProcessUploadedFiles", "Process");
        }


    }

   
    
 }

