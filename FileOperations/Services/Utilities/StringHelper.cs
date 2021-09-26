using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileOperations.Services.Utilities
{
    /// <summary>
    /// A helper class for string operations
    /// </summary>
    public class StringHelper : IStringHelper
    {
        /// <summary>
        /// Gets the default encoded string.
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <param name="index">Index of the first byte</param>
        /// <param name="count">count</param>
        /// <returns>string</returns>
        public string GetDefaultEncodedString(byte[] bytes, int index, int count)
        {
            return Encoding.Default.GetString(bytes, index, count);
        }

        /// <summary>
        /// Gets the unicode encoded string.
        /// </summary>
        /// <param name="bytes">bytes</param>
        /// <param name="index">Index of the first byte</param>
        /// <param name="count">count</param>
        /// <returns>string</returns>
        public string GetUniCodeEncodedString(byte[] bytes, int index, int count)
        {
            return Encoding.Unicode.GetString(bytes, index, count);
        }
    }
}
