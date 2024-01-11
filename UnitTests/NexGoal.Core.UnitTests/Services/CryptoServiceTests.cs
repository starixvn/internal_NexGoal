namespace NexGoal.Core.UnitTests.Services
{
    using NexGoal.Core.Models;
    using NexGoal.Core.Services;
    using Xunit;

    public class CryptoServiceTests
    {
        #region Encrypt

        [Theory]
        [InlineData(null)]
        [InlineData("  ")]
        public void Encrypt_PlainTextIsNullOrWhiteSpace_ReturnEmpty(string plainText)
        {
            // Arrange
            var appSettings = FakeAppSettings(true);
            var service = GetCryptoService(appSettings);

            // Act
            var cipherText = service.Encrypt(plainText);

            // Assert
            Assert.Equal(string.Empty, cipherText);
        }

        [Fact]
        public void Encrypt_DisableEncrypt_ReturnPlainText()
        {
            // Arrange
            var plainText = "123456";
            var appSettings = FakeAppSettings(false);
            var service = GetCryptoService(appSettings);

            // Act
            var cipherText = service.Encrypt(plainText);

            // Assert
            Assert.Equal(plainText, cipherText);
        }

        [Fact]
        public void Encrypt_EnableEncrypt_ExpectedEncryptSuccessfully()
        {
            // Arrange
            var plainText = "123456";
            var appSettings = FakeAppSettings(true);
            var service = GetCryptoService(appSettings);

            // Act
            var cipherText = service.Encrypt(plainText);

            // Assert
            Assert.NotEqual(plainText, cipherText);
        }

        #endregion Encrypt

        #region Decrypt

        [Theory]
        [InlineData(null)]
        [InlineData("  ")]
        public void Decrypt_CipherTextIsNullOrWhiteSpace_ReturnEmpty(string cipherText)
        {
            // Arrange
            var appSettings = FakeAppSettings(true);
            var service = GetCryptoService(appSettings);

            // Act
            var plainText = service.Decrypt(cipherText);

            // Assert
            Assert.Equal(string.Empty, plainText);
        }

        [Fact]
        public void Decrypt_DisableEncrypt_ReturnCipherText()
        {
            // Arrange
            var cipherText = "123456";
            var appSettings = FakeAppSettings(false);
            var service = GetCryptoService(appSettings);

            // Act
            var plainText = service.Decrypt(cipherText);

            // Assert
            Assert.Equal(cipherText, plainText);
        }

        [Fact]
        public void Decrypt_EnableEncrypt_ExpectedDecryptSuccessfully()
        {
            // Arrange
            var expected = "123456";
            var appSettings = FakeAppSettings(true);
            var service = GetCryptoService(appSettings);
            var cipherText = service.Encrypt(expected);

            // Act
            var actual = service.Decrypt(cipherText);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion Decrypt

        #region Helpers

        private static AppSettings FakeAppSettings(bool enableEncrypt)
        {
            var appSettings = new AppSettings
            {
                EnableEncrypt = enableEncrypt
            };

            return appSettings;
        }

        private static CryptoService GetCryptoService(AppSettings appSettings)
        {
            var service = new CryptoService(appSettings);

            return service;
        }

        #endregion Helpers
    }
}