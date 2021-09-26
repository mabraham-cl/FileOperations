using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileOperations.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileOperations.Pages
{
    public class TopFrequentWordsModel : PageModel
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="iFileOperations">IFileOperations two of them injected of types TextFileOperations and BinaryFilOperations.</param>
        public TopFrequentWordsModel(IEnumerable<IFileOperations> iFileOperations)
        {
            _iFileOperations = iFileOperations;
        }

        public void OnGet()
        {
        }

        /// <summary>
        /// Uploaded file 
        /// </summary>
        [BindProperty]
        public IFormFile FormFile { get; set; }

        /// <summary>
        /// Stores the top n frequent words to be displayed in the UI
        /// </summary>
        [BindProperty]
        public Dictionary<string, int> FrequentWords { get; set; }

        /// <summary>
        /// Error message to be displayed for user
        /// </summary>
        [BindProperty]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Post file handler
        /// </summary>
        public void OnPostFile()
        {
            // Creates a zero byte temp file.
            var filePath = Path.GetTempFileName();

            try
            {     
                if (FormFile == null)
                {
                    ErrorMessage = "Please choose a file.";
                    return;
                }

                // Copy to temp file.
                using (var stream = System.IO.File.Create(filePath))
                {
                    FormFile.CopyTo(stream);
                }

                // Gets file extension
                string fileType = Path.GetExtension(FormFile.FileName).TrimStart('.').ToLower();

                IFileOperations currentFileOperations;

                // Invoke the correct (derived class) object based on file extension.

                if (fileType == FileTypes.Txt.ToString().ToLower())
                    currentFileOperations = _iFileOperations.FirstOrDefault(x => x.GetType() == typeof(TextFileOperations));

                else if (fileType == FileTypes.Dat.ToString().ToLower() || fileType == FileTypes.Dll.ToString().ToLower())
                    currentFileOperations = _iFileOperations.FirstOrDefault(x => x.GetType() == typeof(BinaryFileOperations));

                else
                    currentFileOperations = _iFileOperations.FirstOrDefault(x => x.GetType() == typeof(TextFileOperations));

                if (currentFileOperations != null)
                {
                    currentFileOperations.FileName = filePath;

                    // Fetch top n frequent words
                    currentFileOperations.FetchFrequentWords();

                    ErrorMessage = currentFileOperations.ErrorMessage;

                    FrequentWords = currentFileOperations.FrequentWords;
                }
                else
                    ErrorMessage = "File type not supported.";
            }
            catch(Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                try
                {
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }
                catch
                {
                    ErrorMessage = "Unable to delete the file after processing.";
                }
            }
        }

        /// <summary>
        /// Store IFileOperations objects
        /// </summary>
        private IEnumerable<IFileOperations> _iFileOperations;
    }
}
