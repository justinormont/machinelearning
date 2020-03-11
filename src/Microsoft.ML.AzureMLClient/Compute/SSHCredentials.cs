// <copyright file="SSHCredentials.cs" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

namespace Azure.MachineLearning.Services.Compute
{
    public class SSHCredentials
    {
        public SSHCredentials()
        {
            // Nothing to do
        }

        public SSHCredentials(GeneratedOld.Models.VirtualMachineSshCredentials sshCredentialsDto)
        {
            Throw.IfNull(sshCredentialsDto, nameof(sshCredentialsDto));
            this.Username = sshCredentialsDto.Username;
            this.Password = sshCredentialsDto.Password;
            this.PublicKey = sshCredentialsDto.PublicKeyData;
            this.PrivateKey = sshCredentialsDto.PrivateKeyData;
            this.Passphrase = sshCredentialsDto.Passphrase;
        }

        public SSHCredentials(GeneratedOld.Models.UserAccountCredentials userAccountCredentialsDto)
        {
            Throw.IfNull(userAccountCredentialsDto, nameof(userAccountCredentialsDto));

            this.Username = userAccountCredentialsDto.AdminUserName;
            this.Password = userAccountCredentialsDto.AdminUserPassword;
            this.PublicKey = userAccountCredentialsDto.AdminUserSshPublicKey;
            // UserAccountCredentials lacks the following fields
            // but it makes more sense to reuse this class
            this.PrivateKey = default(string);
            this.Passphrase = default(string);
        }

        public string Username { get; set; }

        // See
        // https://github.com/dotnet/platform-compat/blob/master/docs/DE0001.md
        // For why this is not a SecureString
        public string Password { get; set; }

        public string PublicKey { get; set; }

        public string PrivateKey { get; set; }

        public string Passphrase { get; set; }
    }
}
