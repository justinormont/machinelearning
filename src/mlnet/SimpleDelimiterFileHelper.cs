using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.ProgramSynthesis.Utils.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.ML.CLI
{
    internal class SimpleDelimiterFileHelper : ISimpleDelimiterFileHelper
    {
        private readonly static ISimpleDelimiterFileHelper simpleDelimiterFileHelper = new SimpleDelimiterFileHelper();

        public static ISimpleDelimiterFileHelper Instance
        {
            get => SimpleDelimiterFileHelper.simpleDelimiterFileHelper;
        }

        private SimpleDelimiterFileHelper() { }
        
        public bool HasHeader(string filePath)
        {
            var property = Utilities.Utils.GetDataProperties(filePath);
            return property.HasHeader;
        }

        public IReadOnlyList<string> RawColumnNames(string filePath)
        {
            var property = Utilities.Utils.GetDataProperties(filePath);
            if (property.RawColumnNames != null)
            {
                return property.RawColumnNames;
            }
            else
            {
                return Enumerable.Range(0, property.ColumnCount).Select(i => $"col{i.ToString()}").ToList().AsReadOnly();
            }
        }
    }
}
