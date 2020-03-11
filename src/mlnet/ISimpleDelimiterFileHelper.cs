using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ML.CLI
{
    /// <summary>
    /// Helper Interface for Simple Delimiter File
    /// </summary>
    internal interface ISimpleDelimiterFileHelper
    {
        /// <summary>
        /// Detect if input file has header
        /// </summary>
        /// <param name="filePath">full path to input file</param>
        /// <returns></returns>
        bool HasHeader(string filePath);

        /// <summary>
        /// Return Raw Column Name of input file
        /// </summary>
        /// <param name="filePath">full path to input file</param>
        /// <returns>Column Name List, or default Name if input file doesn't have header</returns>
        IReadOnlyList<string> RawColumnNames(string filePath);
    }
}
