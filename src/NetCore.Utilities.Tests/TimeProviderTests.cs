using System;
using System.Globalization;
using Xunit;

namespace ICG.NetCore.Utilities.Tests
{
    public class TimeProviderTests
    {
        private readonly ITimeProvider _timeProvider;

        public TimeProviderTests()
        {
            _timeProvider = new TimeProvider();
        }

        [Fact]
        public void Now_ShouldReturnDateTimeNow()
        {
            var expected = DateTime.Now;
            var result = _timeProvider.Now;
            var difference = expected - result;
            Assert.True(Math.Abs(difference.TotalSeconds) < 1);
        }

        [Fact]
        public void Today_ShouldReturnDateTimeToday()
        {
            var expected = DateTime.Today;
            var result = _timeProvider.Today;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void UtcNow_ShouldReturnDateTimeUtcNow()
        {
            var expected = DateTime.UtcNow;
            var result = _timeProvider.UtcNow;
            var difference = expected - result;
            Assert.True(Math.Abs(difference.TotalSeconds) < 1);
        }

        [Theory]
        [InlineData(1, 2019)]
        [InlineData(2, 2019)]
        [InlineData(3, 2019)]
        [InlineData(4, 2019)]
        [InlineData(5, 2019)]
        [InlineData(6, 2019)]
        [InlineData(7, 2019)]
        [InlineData(8, 2019)]
        [InlineData(9, 2019)]
        [InlineData(10, 2019)]
        [InlineData(11, 2019)]
        [InlineData(12, 2019)]
        public void DaysInMonth_ShouldReturnDateTimeDaysInMonthValue(int month, int year)
        {
            var expectedResult = DateTime.DaysInMonth(year, month);
            var actualResult = _timeProvider.DaysInMonth(year, month);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void Parse_ShouldReturnExpectedDate()
        {
            var input = "5/1/2009 6:32 PM";
            var expected = DateTime.Parse(input);
            var actual = _timeProvider.Parse(input);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_WithProvider_ShouldReturnExpectedDate()
        {
            var input = "01/05/2009 18:32";
            var provider = new CultureInfo("fr-FR");
            var expected = DateTime.Parse(input, provider);
            var actual = _timeProvider.Parse(input, provider);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Parse_WithProviderAndStyles_ShouldReturnExpectedDate()
        {
            var input = "2009-05-01T18:32:00";
            var provider = CultureInfo.InvariantCulture;
            var styles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
            var expected = DateTime.Parse(input, provider, styles);
            var actual = _timeProvider.Parse(input, provider, styles);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TryParse_ShouldReturnSameAsDateTimeTryParse()
        {
            var input = "5/1/2009 6:32 PM";
            var expectedResult = DateTime.TryParse(input, out var expectedOutput);
            var actualResult = _timeProvider.TryParse(input, out var actualOutput);
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void TryParse_WithProviderAndStyles_ShouldReturnSameAsDateTimeTryParse()
        {
            var input = "2009-05-01T18:32:00";
            var provider = CultureInfo.InvariantCulture;
            var styles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
            var expectedResult = DateTime.TryParse(input, provider, styles, out var expectedOutput);
            var actualResult = _timeProvider.TryParse(input, provider, styles, out var actualOutput);
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void TryParseExact_SingleFormat_ShouldReturnExpected()
        {
            var input = "2009-05-01";
            var format = "yyyy-MM-dd";
            var provider = CultureInfo.InvariantCulture;
            var style = DateTimeStyles.None;
            var expectedResult = DateTime.TryParseExact(input, format, provider, style, out var expectedOutput);
            var actualResult = _timeProvider.TryParseExact(input, format, provider, style, out var actualOutput);
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void TryParseExact_MultipleFormats_ShouldReturnExpected()
        {
            var input = "01/05/2009";
            var formats = new[] { "dd/MM/yyyy", "yyyy-MM-dd" };
            var provider = CultureInfo.InvariantCulture;
            var style = DateTimeStyles.None;
            var expectedResult = DateTime.TryParseExact(input, formats, provider, style, out var expectedOutput);
            var actualResult = _timeProvider.TryParseExact(input, formats, provider, style, out var actualOutput);
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void SecondsSinceEpoch_ShouldReturnProperValue()
        {
            var inputDate = new DateTime(2019, 1, 1, 12, 30, 15, DateTimeKind.Utc);
            ulong expectedResult = 1546345815;
            var actualResult = _timeProvider.SecondsSinceEpoch(inputDate);
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void UtcNowSecondsSinceEpoch_ShouldMatchSecondsSinceEpochOfUtcNow()
        {
            var utcNow = _timeProvider.UtcNow;
            var expected = _timeProvider.SecondsSinceEpoch(utcNow);
            var actual = _timeProvider.UtcNowSecondsSinceEpoch();
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertTimeFromUtc_ShouldThrowException_WhenUnknownTimezoneTarget()
        {
            var startDate = DateTime.UtcNow;
            var targetTimezone = "Happy Place";
            Assert.Throws<TimeZoneNotFoundException>(() => _timeProvider.ConvertTimeFromUtc(targetTimezone, startDate));
        }

        [Fact]
        public void ConvertTimeFromUtc_ShouldConvertToLocalTimeZone()
        {
            var utcDate = new DateTime(2022, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var localZone = TimeZoneInfo.Local.Id;
            var expected = TimeZoneInfo.ConvertTimeFromUtc(utcDate, TimeZoneInfo.Local);
            var actual = _timeProvider.ConvertTimeFromUtc(localZone, utcDate);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConvertTimeToUtc_ShouldThrowException_WhenUnknownTimezoneTarget()
        {
            var startDate = DateTime.Now;
            var targetTimezone = "Happy Place";
            Assert.Throws<TimeZoneNotFoundException>(() => _timeProvider.ConvertTimeToUtc(targetTimezone, startDate));
        }

        [Fact]
        public void ConvertTimeToUtc_ShouldConvertFromLocalTimeZone()
        {
            var localDate = new DateTime(2022, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
            var localZone = TimeZoneInfo.Local.Id;
            var expected = TimeZoneInfo.ConvertTimeToUtc(localDate, TimeZoneInfo.Local);
            var actual = _timeProvider.ConvertTimeToUtc(localZone, localDate);
            Assert.Equal(expected, actual);
        }
    }
}