using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace HTML_to_PDF_converter
{
    public static class FileHelper
    {
        public static List<ConvertedFile> GetFilesList(string path)
        {
            try
            {
                var convertedFileList = new List<ConvertedFile>();
                var foldersInfo = new DirectoryInfo(path).GetDirectories();
                foreach (var fld in foldersInfo)
                {
                    var fileInfo = new DirectoryInfo(fld.FullName).GetFiles("*.pdf");
                    foreach (var file in fileInfo)
                    {
                        convertedFileList.Add(new ConvertedFile(fld.Name, file.Name));
                    }
                }
                return convertedFileList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static async Task Convert(string inputUri, string outputPdf)
        {
            try
            {
                var options = new LaunchOptions
                {
                    Headless = true
                };
                using (var browser = await Puppeteer.LaunchAsync(options))
                using (var page = await browser.NewPageAsync())
                {
                    inputUri = new Uri(inputUri).ToString();
                    await page.GoToAsync(inputUri);
                    await page.PdfAsync(outputPdf);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DirectoryInfo SaveFile(Stream stream, string path, string name)
        {
            try
            {
                var directoryInfo = Directory.CreateDirectory(GetNewFolderPath(path));
                var inputFileInfo = Path.Combine(directoryInfo.FullName, name);
                using (var fs = new FileStream(inputFileInfo, FileMode.Create))
                {
                    stream.CopyTo(fs);
                }
                return directoryInfo;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static async Task DeleteAllFiles(string path)
        {
            try
            {
                var folders = new DirectoryInfo(path).GetDirectories();
                foreach (var fld in folders)
                {
                    var files = new DirectoryInfo(fld.FullName).GetFiles();
                    foreach (var f in files)
                    {
                        f.Delete();
                    }
                    fld.Delete();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Dictionary<string, byte[]> GetFileById(string filesFolder, string fileId)
        {
            try
            {
                var filePath = Path.Combine(filesFolder, fileId);
                var pdfFileInfo = new DirectoryInfo(filePath).GetFiles("*.pdf");
                using (var memoryStream = new MemoryStream())
                using (var fileStream = File.OpenRead(pdfFileInfo[0].FullName))
                {
                    fileStream.CopyTo(memoryStream);
                    var result = new Dictionary<string, byte[]>();
                    result.Add(pdfFileInfo[0].Name, memoryStream.ToArray() );
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static string GetNewFolderPath(string path)
        {
            var newFolderName = 1;
            var newFolderPath = string.Empty;
            var folders = new DirectoryInfo(path).GetDirectories().ToList();
            if (folders.Count > 0)
            {
                newFolderName = (int.Parse(folders.Last().Name) + 1);
            }
            newFolderPath = Path.Combine(path, newFolderName.ToString());

            return newFolderPath;
        }
    }
}
