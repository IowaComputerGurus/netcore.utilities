
using Microsoft.Extensions.Options;
using System;
using Xunit;

namespace ICG.NetCore.Utilities.Tests
{
    public class AesEncryptionServiceTests
    {
        private readonly AesEncryptionServiceOptions _options = new AesEncryptionServiceOptions()
        {
            IV = "/EaL5Gx/mKdjYcq4RNecJw==",
            Key = "mqA8ETIup/PjEtLs9vhJnmfZEHYnv98G2umq6UqAmfs="
        };

        private readonly IAesEncryptionService _service;
        public AesEncryptionServiceTests()
        {
            _service = new AesEncryptionService(new OptionsWrapper<AesEncryptionServiceOptions>(_options));
        }

        [Fact]
        public void Encrypt_ShouldThrowArgumentNullException_WhenMissingPlainTextData()
        {
            //Arrange
            var key = "key";
            var iv = "iv";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Encrypt(null, key, iv));
            Assert.Equal("plainTextInput", exception.ParamName);
        }

        [Fact]
        public void Encrypt_ShouldThrowArgumentNullException_WhenMissingKey()
        {
            //Arrange
            var plainText = "Testing";
            var iv = "iv";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Encrypt(plainText, null, iv));
            Assert.Equal("key", exception.ParamName);
        }
        
        [Fact]
        public void Encrypt_ShouldThrowArgumentNullException_WhenMissingIv()
        {
            //Arrange
            var plainText = "Testing";
            var key = "mykey";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Encrypt(plainText, key, null));
            Assert.Equal("iv", exception.ParamName);
        }

        [Fact]
        public void Decrypt_ShouldThrowArgumentNullException_WhenMissingEncryptedTextData()
        {
            //Arrange
            var key = "key";
            var iv = "iv";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Decrypt(null, key, iv));
            Assert.Equal("encryptedInput", exception.ParamName);
        }

        [Fact]
        public void Decrypt_ShouldThrowArgumentNullException_WhenMissingKey()
        {
            //Arrange
            var encryptedText = "Testing";
            var iv = "iv";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Decrypt(encryptedText, null, iv));
            Assert.Equal("key", exception.ParamName);
        }

        [Fact]
        public void Descrypt_ShouldThrowArgumentNullException_WhenMissingIv()
        {
            //Arrange
            var encryptedText = "Testing";
            var key = "mykey";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Decrypt(encryptedText, key, null));
            Assert.Equal("iv", exception.ParamName);
        }

        [Theory]
        [InlineData("msellers@iowacomputergurus.com|1234567890")]
        [InlineData("msellers|1234567890")]
        [InlineData("test|123456789001234578901263348976321325687")]
        public void EncryptedValues_ShouldBeReversable(string input)
        {
            //Act
            var encrypted = _service.Encrypt(input);
            var decrypted = _service.Decrypt(encrypted);

            //Assert
            Assert.Equal(input, decrypted);
        }

        ///<summary>
        ///    This test is need to ensure that stored values, that were encrypted with the known IV/Secret still can be properly decrypted.  
        ///    This was necessary after weird behaviors noticed in .NET 5 -> .NET 6 transition
        /// </summary>
        [Theory]
        [InlineData("b21WiVMxr+V9tlz097TJukwZaUNb3upD44EaZMe4r88Z8CJJyj9DMcQ+Rz1xha2G", "msellers@iowacomputergurus.com|1234567890")]
        [InlineData("AjBzF0gYOERux9RI39D5MaFoM0qyuYFzH4X0rxPEx4k=", "msellers|1234567890")]
        [InlineData("u5mWzbtkZk3Hou8ZNnmhQWej4YYCsioRR7QwHKcBV8AvMw1F9JURogGcX94Fj/jK", "test|123456789001234578901263348976321325687")]
        public void Decrypt_ShouldProperlyDecrypt(string encryptedValue, string expectedValue)
        {
            //Arrange

            //Act
            var decrypted = _service.Decrypt(encryptedValue);

            //Assert
            Assert.Equal(expectedValue, decrypted);
        }
    }
}
