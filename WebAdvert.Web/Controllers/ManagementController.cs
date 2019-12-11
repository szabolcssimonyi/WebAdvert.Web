using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using WebAdvert.Web.Interfaces;
using WebAdvert.Web.Models.Management;

namespace WebAdvert.Web.Controllers
{
    public class ManagementController : Controller
    {
        private readonly IFileUploader fileUploader;

        public ManagementController(IFileUploader fileUploader)
        {
            this.fileUploader = fileUploader;
        }

        [Route("create")]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("InvalidModel", "Model is invalid");
                return View(model);
            }
            var id = "1111";
            var fileName = string.IsNullOrEmpty(imageFile.FileName) ? imageFile.FileName : id;
            var path = Path.Combine(id, fileName);
            try
            {
                using (var readStream = imageFile.OpenReadStream())
                {
                    var result = await fileUploader.UploadFileAsync(path, readStream);
                    if (!result)
                    {
                        throw new Exception("Could not upload image to file repository. Please see the logs for details");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}