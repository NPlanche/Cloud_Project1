using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proxy.Controllers
{
    /// <summary>
    /// HomeController
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        IConfiguration _configuration;

        public ApiController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return StatusCode(200, "Listening...");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Upload
        /// reference
        /// https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads?view=aspnetcore-7.0
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file.Length > 0)
                {
                    var path = _configuration["Folder"].ToString();

                    bool exists = Directory.Exists(path);

                    if (!exists)
                    {
                        Directory.CreateDirectory(path);
                    }

                    var filePath = $"{path}{file.FileName}";

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// GetFiles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetFiles")]
        public IActionResult GetFiles()
        {
            try
            {
                string[] files = GetFileNames(_configuration["Folder"].ToString(), "*.*");

                return StatusCode(200, files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// GetFile
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetFile/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            try
            {
                string[] files = GetFileNames(_configuration["Folder"].ToString(), "*.*");

                var file = files.SingleOrDefault(p => p == fileName);

                return StatusCode(200, file);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// GetFileNames
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        private static string[] GetFileNames(string path, string filter)
        {
            string[] files = Directory.GetFiles(path, filter);

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = Path.GetFileName(files[i]);
            }

            return files;
        }
    }
}
