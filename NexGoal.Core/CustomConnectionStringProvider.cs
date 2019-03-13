namespace NexGoal.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using Fanex.Data;
    using Microsoft.Extensions.Options;
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;

    public class CustomConnectionStringProvider : ConnectionStringProviderBase
    {
        private const string NotFoundConnectionString = "Could not read the connection strings.";
        private const string InvalidPassword = "Could not read the password.";

        private readonly ICryptoService cryptoService;
        private readonly int algorithm;

        public CustomConnectionStringProvider(
            Dictionary<string, string> connections,
            int algorithm,
            IOptionsMonitor<AppSettings> configuration)
        {
            this.algorithm = algorithm;
            cryptoService = new CryptoService(configuration);

            ConnectionMaps = connections;
            PasswordDecryptor = DecryptConnectionString;
        }

        public string DecryptConnectionString(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString), NotFoundConnectionString);
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);

            switch (algorithm)
            {
                case 0:
                    connectionStringBuilder.Password = Cryptography.DecryptPassword(connectionStringBuilder.Password);
                    break;

                case 1:
                    connectionStringBuilder.Password = DecryptPassword(connectionStringBuilder.Password);
                    break;

                default:
                    // nothing
                    break;
            }

            return connectionStringBuilder.ToString();
        }

        public string DecryptPassword(string encryptedPassword)
        {
            if (encryptedPassword == null)
            {
                throw new ArgumentNullException(nameof(encryptedPassword), InvalidPassword);
            }

            var plainPassword = cryptoService.Decrypt(encryptedPassword);

            return plainPassword;
        }
    }
}