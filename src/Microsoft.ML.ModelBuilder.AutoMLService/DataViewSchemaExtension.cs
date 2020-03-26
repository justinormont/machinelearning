// <copyright file="DataViewSchemaExtension.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using Microsoft.Data;
using Microsoft.ML.Data;

namespace Microsoft.ML.ModelBuilder.AutoMLService
{
    public static class DataViewSchemaExtension
    {
        public static BaseColumn ToBaseColumn(this DataViewSchema.Column column, object value)
        {
            if (column.Type is TextDataViewType)
            {
                var columns = new StringColumn(column.Name, 0);
                columns.Append(value as string);
                return columns;
            }
            else if (column.Type.RawType == typeof(bool))
            {
                var primitiveColumn = new PrimitiveColumn<bool>(column.Name);
                try
                {
                    primitiveColumn.Append(value != null && (string)value != string.Empty ? Convert.ToBoolean(value) : false);
                }
                catch
                {
                    throw new InvalidCastException(string.Format("Input string for {0} is not in the correct format.", column.Name));
                }

                return primitiveColumn;
            }
            else if (column.Type.RawType == typeof(int))
            {
                var primitiveColumn = new PrimitiveColumn<int>(column.Name);
                try
                {
                    primitiveColumn.Append(value != null && (string)value != string.Empty ? Convert.ToInt32(value) : 0);
                }
                catch
                {
                    throw new InvalidCastException(string.Format("Input string for {0} is not in the correct format.", column.Name));
                }

                return primitiveColumn;
            }
            else if (column.Type.RawType == typeof(float))
            {
                var primitiveColumn = new PrimitiveColumn<float>(column.Name);
                try
                {
                    primitiveColumn.Append(value != null && (string)value != string.Empty ? Convert.ToSingle(value) : 0);
                }
                catch
                {
                    throw new InvalidCastException(string.Format("Input string for {0} is not in the correct format.", column.Name));
                }

                return primitiveColumn;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
