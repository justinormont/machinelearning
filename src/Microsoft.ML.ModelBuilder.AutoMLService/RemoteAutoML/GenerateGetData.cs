// <copyright file="GenerateGetData.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using Microsoft.Azure.Management.BatchAI.Fluent.Models;

namespace AzureML
{
    internal static class GenerateGetData
    {
        public static void GenerateGetDataPy(
            string dataRefName,
            string trainingFileName,
            string labelColumnName,
            string outputFile)
        {
            string pyTemplate =
$@"from sklearn import datasets
import argparse
import os
import urllib
import numpy as np
import pandas as pd

def get_data():
    data_folder = os.environ[""AZUREML_DATAREFERENCE_{dataRefName}""]
    print('Data folder:', data_folder)

    data = pd.read_csv(os.path.join(data_folder, '{trainingFileName}'), delimiter = '\t', skipinitialspace = True) #, header=None)

    y = data.pop('{labelColumnName}')
    y = y.values
    X = data

    return {{ ""X"": X, ""y"": y}}
";

            System.IO.File.WriteAllText(outputFile, pyTemplate);
        }
    }
}