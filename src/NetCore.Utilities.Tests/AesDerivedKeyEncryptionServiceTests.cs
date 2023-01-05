using System;
using Microsoft.Extensions.Options;
using Xunit;

namespace ICG.NetCore.Utilities.Tests
{
    public class AesDerivedKeyEncryptionServiceTests
    {
        private readonly AesDerivedKeyEncryptionServiceOptions _options = new AesDerivedKeyEncryptionServiceOptions()
        {
            Passphrase = "This Is My Secret"
        };

        private readonly IAesDerivedKeyEncryptionService _service;

        public AesDerivedKeyEncryptionServiceTests()
        {
            _service = new AesDerivedKeyEncryptionService(new OptionsWrapper<AesDerivedKeyEncryptionServiceOptions>(_options));
        }

        [Fact]
        public void Encrypt_ShouldThrowArgumentNullException_WhenMissingPlainTextData()
        {
            //Arrange
            var salt = "testSalt";
            
            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Encrypt(null, salt));
            Assert.Equal("plainTextInput", exception.ParamName);
        }

        [Fact]
        public void Encrypt_ShouldThrowArgumentNullException_WhenMissingSaltValue()
        {
            //Arrange
            var plainText = "Testing";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Encrypt(plainText, null));
            Assert.Equal("salt", exception.ParamName);
        }

        [Fact]
        public void Encrypt_ShouldThrowArgumentNullException_WhenMissingPassphrase()
        {
            //Arrange
            var plainText = "Testing";
            var salt = "Salt";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Encrypt(plainText, salt, null));
            Assert.Equal("passphrase", exception.ParamName);
        }

        [Fact]
        public void Decrypt_ShouldThrowArgumentNullException_WhenMissingEncryptedData()
        {
            //Arrange
            var salt = "testSalt";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Decrypt(null, salt));
            Assert.Equal("encryptedInput", exception.ParamName);
        }

        [Fact]
        public void Decrypt_ShouldThrowArgumentNullException_WhenMissingSaltValue()
        {
            //Arrange
            var encryptedText = "Testing";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Decrypt(encryptedText, null));
            Assert.Equal("salt", exception.ParamName);
        }

        [Fact]
        public void Decrypt_ShouldThrowArgumentNullException_WhenMissingPassphrase()
        {
            //Arrange
            var encryptedText = "Testing";
            var salt = "Salt";

            //Act
            var exception = Assert.Throws<ArgumentNullException>(() => _service.Decrypt(encryptedText, salt, null));
            Assert.Equal("passphrase", exception.ParamName);
        }

        [Theory]
        [InlineData("msellers@iowacomputergurus.com|1234567890", "ABC-12354")]
        [InlineData("msellers|1234567890", "ABC-56789")]
        [InlineData("test|123456789001234578901263348976321325687", "Super-Simple-Salt-Value")]
        public void EncryptAndDecryptShouldRoundTripProperly(string input, string salt)
        {
            //Act
            var encrypted = _service.Encrypt(input, salt);
            var decrypted = _service.Decrypt(encrypted, salt);

            //Assert
            Assert.Equal(input, decrypted);
        }

        /////<summary>
        /////    This test is need to ensure that stored values, that were encrypted with the known IV/Secret still can be properly decrypted.  
        /////    This was necessary after weird behaviors noticed in .NET 5 -> .NET 6 transition
        ///// </summary>
        //[Theory]
        //[InlineData("b21WiVMxr+V9tlz097TJukwZaUNb3upD44EaZMe4r88Z8CJJyj9DMcQ+Rz1xha2G", "msellers@iowacomputergurus.com|1234567890")]
        //[InlineData("AjBzF0gYOERux9RI39D5MaFoM0qyuYFzH4X0rxPEx4k=", "msellers|1234567890")]
        //[InlineData("u5mWzbtkZk3Hou8ZNnmhQWej4YYCsioRR7QwHKcBV8AvMw1F9JURogGcX94Fj/jK", "test|123456789001234578901263348976321325687")]
        //public void Decrypt_ShouldProperlyDecrypt(string encryptedValue, string expectedValue)
        //{
        //    //Arrange

        //    //Act
        //    var decrypted = _service.Decrypt(encryptedValue);

        //    //Assert
        //    Assert.Equal(expectedValue, decrypted);
        //}
    }
}
