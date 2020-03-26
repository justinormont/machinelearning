// <copyright file="SSLConfiguration.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

using System;

namespace Azure.MachineLearning.Services.Compute
{
    public class SSLConfiguration
    {
        public SSLConfiguration(GeneratedOld.Models.SslConfiguration sslConfigurationDto = null)
        {
            if (sslConfigurationDto != null)
            {
                this.Status = sslConfigurationDto.Status;
                this.Certificate = sslConfigurationDto.Cert;
                this.Key = sslConfigurationDto.Key;
                this.CNAME = sslConfigurationDto.Cname;
            }
        }

        public SSLConfiguration(string certFile, string keyFile) =>
            throw new NotImplementedException("Cannot configure from cert files yet");

        public string Status { get; private set; }

        public string Certificate { get; private set; }

        public string Key { get; private set; }

        public string CNAME { get; private set; }

        public GeneratedOld.Models.SslConfiguration ToDTO()
        {
            var result = new GeneratedOld.Models.SslConfiguration();
            result.Status = this.Status;
            result.Cert = this.Certificate;
            result.Key = this.Key;
            result.Cname = this.CNAME;

            return result;
        }
    }
}
