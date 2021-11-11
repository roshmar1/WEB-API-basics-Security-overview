using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task1;
using System.Text.Json;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace WEB_API.Controllers
{  
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {
        private static Editor editor = new Editor();

        private string filePath = $@"{Directory.GetParent(Directory.GetCurrentDirectory())}\Storage\";
        
        private readonly ILogger<FileController> _logger;

        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }


        /* public IEnumerable<WeatherForecast> Get()
         {
             var rng = new Random();
             return Enumerable.Range(1, 5).Select(index => new WeatherForecast
             {
                 Date = DateTime.Now.AddDays(index),
                 TemperatureC = rng.Next(-20, 55),
                 Summary = Summaries[rng.Next(Summaries.Length)]
             })
             .ToArray();
         }*/


        [HttpGet]
        [Route("/File/GetFiles")]
        public IActionResult GetFiles()
        {
            string[] fileNames = editor.GetFileNameInStorage();

            return new JsonResult(fileNames);
        }

        [HttpGet]
        [Route("/File/GetFile/{filename}")]
        public IActionResult GetFile(string filename,[FromQuery] string searchText)
        {
            string fullPath = $@"{filePath}{filename}";
            string[] paragraphs = editor.SearchParagraphs(fullPath,searchText);
        
            return new JsonResult(paragraphs);
        }

        [HttpPost]
        [Route("/File/PostFile/{filename}")]
        public IActionResult PostFile(string filename, [FromQuery] string searchText,[FromQuery] string replaceText)
        {
            string fullPath = $@"{filePath}{filename}";
            int paragraphs = editor.FindAndReplace(fullPath, searchText,replaceText);

            return new JsonResult(paragraphs);
        }

        [HttpPut]
        [Route("/File/PutFile/{filename}")]
        public IActionResult PutFile(string filename, IFormCollection data)
        {
            string filePath = $@"{Directory.GetCurrentDirectory()}\Help{filename}";

            using (FileStream fs = System.IO.File.Create(filePath))
            {
                data.Files[0].CopyTo(fs);
            };

            editor.CopyFileToStorage(filePath);

            System.IO.File.Delete(filePath);

            return new JsonResult("OK");
        }
    }
}
