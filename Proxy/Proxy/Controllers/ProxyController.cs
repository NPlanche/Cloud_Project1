using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Proxy.Controllers
{
    /// <summary>
    /// HomeController
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ProxyController : ControllerBase
    {
        IConfiguration _configuration;

        public ProxyController(IConfiguration configuration)
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
        /// https://stackoverflow.com/questions/39397278/post-files-from-asp-net-core-web-api-to-another-asp-net-core-web-api
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("UploadFile")]
        public IActionResult UploadFile(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    string endPointAPI = _configuration["EndPointAPI"].ToString();

                    using (var client = new HttpClient())
                    {
                        try
                        {
                            client.BaseAddress = new Uri(endPointAPI);

                            byte[] data;

                            using (var br = new BinaryReader(file.OpenReadStream()))
                            {
                                data = br.ReadBytes((int)file.OpenReadStream().Length);
                            }

                            ByteArrayContent bytes = new ByteArrayContent(data);

                            MultipartFormDataContent multiContent = new MultipartFormDataContent();

                            multiContent.Add(bytes, "file", file.FileName/*.Replace(".jpg", ".myjpg")*/.Replace(".txt",".mytxt"));

                            var result = client.PostAsync("Upload", multiContent).Result;
                            
                            return StatusCode((int)result.StatusCode);

                        }
                        catch (Exception ex)
                        {
                            return StatusCode(500, ex.Message);
                        }
                    }
                }

                return StatusCode(400, "Bad Request");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Files
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetFiles")]
        public async Task<IActionResult> GetFiles()
        {
            try
            {
                string endPointAPI = _configuration["EndPointAPI"].ToString();

                using (var client = new HttpClient())
                {
                    try
                    {
                        client.BaseAddress = new Uri(endPointAPI);

                        var responseTask = await client.GetAsync("GetFiles");

                        if (responseTask.IsSuccessStatusCode)
                        {
                            var apiResp = await responseTask.Content.ReadAsStringAsync();

                            return StatusCode((int)responseTask.StatusCode, apiResp);
                        }
                        else
                        {
                            return StatusCode((int)responseTask.StatusCode, "something when wrong");
                        }
                    }
                    catch (Exception ex)
                    {
                        return StatusCode(500, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Files
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetFile/{fileName}")]
        public IActionResult GetFile(string fileName)
        {
            try
            {
                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
