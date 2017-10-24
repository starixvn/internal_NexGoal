namespace NexGoal.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using Fanex.Data;
    using NexGoal.Core.Services;
    using Resources;

    public class CustomConnectionStringProvider : ConnectionStringProviderBase
    {
        private readonly ICertificateService certificateService = new CertificateService();

        public CustomConnectionStringProvider()
        {
            ConnectionMaps = ReadConnectionStrings();
            PasswordDecryptor = DecryptConnectionString;
        }

        public string DecryptConnectionString(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(Message.NotFoundConnectionString);
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            connectionStringBuilder.Password = DecryptPassword(connectionStringBuilder.Password);

            return connectionStringBuilder.ToString();
        }

        private Dictionary<string, string> ReadConnectionStrings()
        {
            if (ConfigurationManager.ConnectionStrings == null)
            {
                throw new ConfigurationErrorsException(Message.NotFoundConnectionString);
            }

            IEnumerable<ConnectionStringSettings> connections = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>();

            return connections
                .Where(connection => !string.IsNullOrEmpty(connection.ConnectionString))
                .ToDictionary(connection => connection.Name, connection => connection.ConnectionString, StringComparer.OrdinalIgnoreCase);
        }

        public string DecryptPassword(string encryptedPassword)
        {
            if (encryptedPassword == null)
            {
                throw new ArgumentNullException(Message.InvalidPassword);
            }

            var plainPassword = certificateService.Decrypt(encryptedPassword);

            return plainPassword;
        }
    }
}