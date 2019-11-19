using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace MyWebApp.Helpers
{
    public static class FileHelper
    {
        public static void Create(string path, IFormFile file)
        {
            Directory.CreateDirectory(path);

            var outputFile = File.Create($@"{path}\{file.FileName}");
            var m = new MemoryStream();
            try
            {
                file.CopyTo(m);
                var fileBytes = m.ToArray();
                outputFile.Write(fileBytes, 0, fileBytes.Length);
            }
            catch (Exception e)
            {
            }
            finally
            {
                outputFile.Close();
                m.Close();
            }
        }
    }
}