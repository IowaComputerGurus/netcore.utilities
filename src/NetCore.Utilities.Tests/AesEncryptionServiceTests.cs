
using Microsoft.Extensions.Options;
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
