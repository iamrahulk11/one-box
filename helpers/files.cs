using iText.Kernel.Pdf;
using Microsoft.AspNetCore.Http;

namespace helpers
{
    public record files
    {
        public async Task<string> read(string filePath)
        {
            string fileContent = "";
            try
            {
                StreamReader streamReaderBody = new StreamReader(filePath);
                fileContent = await streamReaderBody.ReadToEndAsync();
                streamReaderBody.Close();
                streamReaderBody.Dispose();
            }
            catch
            {
                throw;
            }
            return fileContent;
        }

        public string saveFromBase64(string base64, string fileName, string pathToSave)
        {
            // Convert the base64 string to a byte array
            byte[] _fileByte = Convert.FromBase64String(base64);

            //checking if directory is present or not.
            if (!Directory.Exists($"{pathToSave}"))
            {
                Directory.CreateDirectory($"{pathToSave}");
            }

            //saving the file 
            File.WriteAllBytes($"{pathToSave}{fileName}", _fileByte);

            return fileName;
        }
        public async Task<(bool, string)> save(IFormFile _fileToSave, string fileName, string pathToSave)
        {
            //saving the file in the directory
            (bool, string) savingTupleResponse = await saveFileInDirectory(_fileToSave, pathToSave, fileName);
            if (savingTupleResponse.Item1 == false)
            {
                return (false, savingTupleResponse.Item2);
            }

            return (true, fileName);
        }
        private async Task<(bool, string)> saveFileInDirectory(IFormFile _fileToSave, string pathToSave, string fileName)
        {
            //checking if directory is present or not.
            if (!Directory.Exists($"{pathToSave}"))
            {
                Directory.CreateDirectory($"{pathToSave}");
            }

            //saving the file 
            using (var stream = new FileStream($"{pathToSave}{fileName}", FileMode.Create))
            {
                await _fileToSave.CopyToAsync(stream);
            }


            if (Path.GetExtension(_fileToSave.FileName.ToLower()) == ".pdf")
            {
                if (IsPdfPasswordProtected($"{pathToSave}{fileName}"))
                {
                    return (false, "Document is password-protected, kindly submit unprotected version.");
                }
            }

            return (true, fileName);
        }
        private static bool IsPdfPasswordProtected(string path)
        {
            bool isPasswordProtected = false;
            try
            {
                using (PdfReader reader = new PdfReader(path))
                {
                    using (PdfDocument pdfDoc = new PdfDocument(reader))
                    {
                        isPasswordProtected = false;
                    }
                }
            }
            catch (Exception)
            {
                isPasswordProtected = true;
                File.Delete(path);
            }
            return isPasswordProtected;
        }
    }
}
