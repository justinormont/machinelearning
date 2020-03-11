// <copyright file="IntOrString.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.ML.CLI.Commands
{
    /// <summary>
    /// For mix-type option in Commands, like LabelCol, which can be both index or string
    /// </summary>
    internal sealed class IntOrString
    {
        private int _int = default;
        private string _str = default;

        private string _rawInput = default;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntOrString"/> class.
        /// Commandline API can parse custom type iif custom type has constructor that accept single string as parameter.
        /// </summary>
        /// <param name="str">INT or STR</param>
        public IntOrString(string str)
        {
            int index;
            this._rawInput = str;

            // if str can be parse to int, treat value as int
            // else treat value as str
            if (int.TryParse(str, out index))
            {
                this._int = index;
            }
            else
            {
                this.IsSTR = true;
                this._str = str;
            }
        }

        public int INT
        {
            get
            {
                if (this.IsSTR)
                {
                    throw new Exception("invalid type");
                }

                return this._int;
            }
        }

        public string STR
        {
            get
            {
                if (!this.IsSTR)
                {
                    throw new Exception("invalid type");
                }

                return this._str;
            }
        }

        public bool IsSTR { get; private set; }

        public override string ToString()
        {
            return this._rawInput;
        }
    }
}
