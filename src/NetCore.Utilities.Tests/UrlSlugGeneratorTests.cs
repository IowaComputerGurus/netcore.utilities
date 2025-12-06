using System;
using Xunit;

namespace ICG.NetCore.Utilities.Tests
{
    /// <summary>
    /// Unit tests for <see cref="UrlSlugGenerator"/>.
    /// </summary>
    public class UrlSlugGeneratorTests
    {
        private readonly UrlSlugGenerator _slugGenerator;

        public UrlSlugGeneratorTests()
        {
            _slugGenerator = new UrlSlugGenerator();
        }

        [Theory]
        [InlineData("Hello World!", "hello-world")]
        [InlineData("C# is awesome!", "c-is-awesome")]
        [InlineData("Multiple   spaces", "multiple-spaces")]
        [InlineData("Special_chars*&^%$#@!", "special-chars")]
        [InlineData("Already-slugified", "already-slugified")]
        [InlineData("UPPERCASE", "uppercase")]
        [InlineData("123 Numbers 456", "123-numbers-456")]
        [InlineData("----Leading and trailing----", "leading-and-trailing")]
        [InlineData("MiXeD CaSe & Symbols!", "mixed-case-symbols")]
        [InlineData("a", "a")]
        [InlineData("A", "a")]
        [InlineData("1", "1")]
        public void GenerateSlug_ValidInput_ReturnsExpectedSlug(string input, string expected)
        {
            var result = _slugGenerator.GenerateSlug(input);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void GenerateSlug_InvalidInput_ThrowsArgumentException(string input)
        {
            Assert.Throws<ArgumentException>(() => _slugGenerator.GenerateSlug(input));
        }

        [Fact]
        public void GenerateSlug_RemovesMultipleDashes()
        {
            var input = "Test---Slug---Generator";
            var expected = "test-slug-generator";
            var result = _slugGenerator.GenerateSlug(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void GenerateSlug_TrimsLeadingAndTrailingDashes()
        {
            var input = "---Trim---Me---";
            var expected = "trim-me";
            var result = _slugGenerator.GenerateSlug(input);
            Assert.Equal(expected, result);
        }
    }
}