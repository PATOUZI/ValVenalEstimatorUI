using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using FileUpload_Core.Models;
using FileUpload_Core.ViewModels;
using Newtonsoft.Json;
using System.Dynamic;

namespace FileUpload_Core.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            List<string> PrefectureList = new List<string>();
            dynamic model = new ExpandoObject();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://localhost:5004/api/Places/prefecture"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    PrefectureList = JsonConvert.DeserializeObject<List<string>>(apiResponse);
                }
            }
            return View(PrefectureList);
        }

        public IActionResult FileUpload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> FileUpload(IFormFile file)
        {
            await UploadFile(file);
            TempData["msg"] = "File Uploaded successfully.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetValVenal(string prefect, string dist, int area)
        {
            string accessPath = @"https://localhost:5004/api/Places/" + prefect + "/" + dist + "/" + area ;
            ValVenalDTO ValVenalDTO = new ValVenalDTO();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(accessPath))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        ValVenalDTO = JsonConvert.DeserializeObject<ValVenalDTO>(apiResponse);   
                    }
                    else
                        ViewBag.StatusCode = response.StatusCode;
                }
            }
            /*Console.WriteLine("Prefecture : " + prefect);
            Console.WriteLine("Quartier : " + dist);
            Console.WriteLine("Superficie : " + area);*/
            ValVenalDTO.Prefecture = prefect;
            ValVenalDTO.District = dist;
            ValVenalDTO.Area = area;
            //Console.WriteLine(ValVenalDTO.ValVenal);
            return View(ValVenalDTO);
        }

        // Upload file on server
        public async Task<bool> UploadFile(IFormFile file)
        {
            string path = "";
            bool iscopied = false;
            try
            {
                if (file.Length > 0)
                {
                    string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Upload"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        await file.CopyToAsync(filestream);
                    }
                    iscopied = true;
                    string filePath = Path.Combine(path, filename);
                    using (var httpClient = new HttpClient())
                    {
                        /*string accessPath = @"https://localhost:5004/api/Places/LoadDataInDb?accessPath=" + filePath;
                        await httpClient.GetAsync(accessPath);*/

                        //https://localhost:5004/api/Places/ + filePath Ne fonctionne pas avec cet URL

                        string accessPath = @"https://localhost:5004/api/Places/LoadDataInDataBase?accessPath=" + filePath;
                        var stringContent = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("field1", "value1"),
                            new KeyValuePair<string, string>("field2", "value2"),
                        });
                        await httpClient.PostAsync(accessPath, stringContent);
                    }
                }
                else
                {
                    iscopied = false;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return iscopied;
        }
    }
}
