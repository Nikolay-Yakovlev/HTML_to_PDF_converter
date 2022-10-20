using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace HTML_to_PDF_converter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConverterController : Controller
    {
        public ConverterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private IConfiguration _configuration;
        private string FilesFolder
        {
            get
            {
                return _configuration.GetSection("FilesFolder").Value;
            }
        }

        [HttpGet]
        public JsonResult Get()
        {
            var dir = new DirectoryInfo(FilesFolder);
            if (!dir.Exists)
            {
                try
                {
                    Directory.CreateDirectory(FilesFolder);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
            var convertedFileList = FileHelper.GetFilesList(FilesFolder);
            return new JsonResult(convertedFileList);

        }

        [HttpGet("{fileId}")]
        public IActionResult Get(string fileId)
        {
            if (!string.IsNullOrEmpty(fileId))
            {
                var contentType = "application/pdf";
                var file = FileHelper.GetFileById(FilesFolder, fileId);
                return File(file.First().Value, contentType, file.First().Key);
            }
            return BadRequest();
        }

        [HttpDelete]
        public async Task DeleteAllFiles()
        {
            await FileHelper.DeleteAllFiles(FilesFolder);
        }

        [HttpPost]
        public async Task Post(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    var newDirInfo = FileHelper.SaveFile(file.OpenReadStream(), FilesFolder, file.FileName);
                    var fileNameWOext = Path.GetFileNameWithoutExtension(file.FileName);
                    var inputHtmlUri = Path.Combine(newDirInfo.FullName, file.FileName);
                    var outputPdfUri = Path.Combine(newDirInfo.FullName, string.Concat(fileNameWOext, ".pdf"));
                    await FileHelper.Convert(inputHtmlUri, outputPdfUri);
                    new FileInfo(inputHtmlUri).Delete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
