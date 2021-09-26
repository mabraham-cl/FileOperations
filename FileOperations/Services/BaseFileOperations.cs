using System;
using System.Collections.Generic;
using System.IO;

namespace FileOperations.Services
{
    /// <summary>
    /// Base class for file operations. Any filetypes can extend this class and have its own implementations.
    /// We can have abstract or virtual members in this class based on the requirements for various file operations.
    /// Right now there is only one file operation added in this class i.e. FetchFrequentWords.
    /// So different type of files can have different implementations of the same method.
    /// The common behaviors are added as virtual in this class e.g: FileName, few common Validation etc
    /// </summary>
    public abstract class BaseFileOperations : IFileOperations
    {
        /// <summary>
        /// Full path of the file. This is set from the client.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Frequent words. Key is the word and value is the count. The set is allowed only in this class or derived class.
        /// </summary>
        public Dictionary<string, int> FrequentWords { get; protected set; }

        /// <summary>
        /// To store error message. The set is allowed only in this class or derived class.
        /// </summary>
        public string ErrorMessage { get; protected set; }

        /// <summary>
        /// Validate the input file. This can be overriden by a derived class to include its own validations
        /// </summary>
        /// <returns>Returns true or false</returns>
        protected virtual bool Validate()
        {
            if (string.IsNullOrEmpty(FileName) || string.IsNullOrWhiteSpace(FileName))
            {
                ErrorMessage = "Please upload file.";
                return false;
            }

            else if (!File.Exists(FileName))
            {
                ErrorMessage = "File does not exists.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Check if the input is valid
        /// </summary>
        /// <returns>True if validation succeeded. Otherwise return false.</returns>
        protected bool IsValid()
        {
            // Validation method is called here. So derived classes only need to call IsValid().
            return Validate();
        }        

        /// <summary>
        /// This will fetch the top n frequent words used in the file.
        /// </summary>
        public abstract void FetchFrequentWords();
    }
}
