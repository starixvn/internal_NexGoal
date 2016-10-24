namespace NexGoal.Core.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.SqlClient;
    using System.Linq;
    using Fanex.Data;
    using NexGoal.Core.Services;

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
                throw new ArgumentNullException("Could not read the connection strings");
            }

            var connectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
            connectionStringBuilder.Password = DecryptPassword(connectionStringBuilder.Password);

            return connectionStringBuilder.ToString();
        }

        private Dictionary<string, string> ReadConnectionStrings()
        {
            if (ConfigurationManager.ConnectionStrings == null)
            {
                throw new ConfigurationErrorsException("Could not read the connection strings from your web.config or app.config.");
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
                throw new ArgumentNullException("Could not read the password");
            }

            var plainPassword = certificateService.Decrypt(encryptedPassword);

            return plainPassword;
        }
    }
}