// <copyright file="SecretValueAttribute.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

/*
 * This file is copied from
 * Vienna/src/azureml-api/src/Common/Core/Utilities/SecretValueAttribute.cs
 * This is a temporary solution until we can get the Swagger files updated
 * */

namespace Azure.MachineLearning.Services
{
    // An attribute for marking a property as a secret property. like password or key properties.
    // If this attribute is applied to a property then the value of that property is a secret value,
    // which we may need to retrieve from the credential service.
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class SecretValueAttribute : Attribute
    {
        // No field needed for now.
    }
}
