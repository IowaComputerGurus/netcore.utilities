using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace ICG.NetCore.Utilities.Tests
{
    public class EnumExtensionsTests
    {
        [Fact]
        public void GetDisplayName_ShouldReturnDisplayName_WhenAttributeFound()
        {
            //Arrange
            var value = TestEnum.FormattedValue;
            var expectedLabel = "Testing";

            //Act
            var result = value.GetDisplayName();

            //Assert
            Assert.Equal(expectedLabel, result);
        }

        [Fact]
        public void GetDisplayName_ShouldThrowNullReferenceException_WhenAttributeNotFound()
        {
            //Arrange
            var value = TestEnum.CleanValue;

            //Act
            var recordData = Record.Exception(() => value.GetDisplayName());

            //Assert
            Assert.NotNull(recordData);
            Assert.IsType<NullReferenceException>(recordData);
        }

        [Theory]
        [InlineData(TestEnum.CleanValue, false)]
        [InlineData(TestEnum.FormattedValue, true)]
        public void HasDisplayName_ShouldReturnProperBoolValue(TestEnum value, bool expectedResult)
        {
            //Arrange

            //Act
            var result = value.HasDisplayName();

            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Theory]
        [InlineData(TestEnum.CleanValue, "CleanValue")]
        [InlineData(TestEnum.FormattedValue, "Testing")]
        public void GetDisplayNameOrStringValue_ShouldReturnDisplayName_OrEnumValue_WithoutException(TestEnum value,
            string expectedResult)
        {
            //Arrange

            //Act
            var result = value.GetDisplayNameOrStringValue();

            //Assert
            Assert.Equal(expectedResult, result);
        }
    }
}
