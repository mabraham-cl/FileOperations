using System.Collections.Generic;

namespace FileOperations.Services
{
    /// <summary>
    /// Public interface to Fileoperations
    /// </summary>
    public interface IFileOperations
    {
        /// <summary>
        /// Gets the  errormessage
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Full path of the file. This is set from the client.
        /// </summary>
        string FileName { get; set; }

        /// <summary>
        /// Gets FrequentWords
        /// </summary>
        Dictionary<string, int> FrequentWords { get; }

        /// <summary>
        /// Finds the top n frequentwords and its count
        /// </summary>
        void FetchFrequentWords();
    }
}