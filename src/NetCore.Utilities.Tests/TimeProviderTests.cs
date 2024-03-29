﻿using System;
using System.Runtime.InteropServices;
using ICG.NetCore.Utilities;
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
            //Arrange
            var expected = DateTime.Now;

            //Act
            var result = _timeProvider.Now;

            //Assert
            //Due to clock times, make sure that we are within 1 second
            var difference = expected - result;
            Assert.True(difference.Milliseconds < 1000);
        }

        [Fact]
        public void Today_ShouldReturnDateTimeToday()
        {
            //Arrange
            var expected = DateTime.Today;

            //Act
            var result = _timeProvider.Today;

            //Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void UtcNow_ShouldReturnDateTimeUtcNow()
        {
            //Arrange
            var expected = DateTime.UtcNow;

            //Act
            var result = _timeProvider.UtcNow;

            //Assert
            //Due to clock times, make sure we are within 1 second
            var difference = expected - result;
            Assert.True(difference.Milliseconds < 1000);
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
            //Arrange
            var expectedResult = DateTime.DaysInMonth(year, month);

            //Act
            var actualResult = _timeProvider.DaysInMonth(year, month);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void TryParse_ShouldReturnSameAsDateTimeTryParse()
        {
            //Arrange
            var input = "5/1/2009 6:32 PM";
            var expectedResult = DateTime.TryParse(input, out var expectedOutput);

            //Act
            var actualResult = _timeProvider.TryParse(input, out var actualOutput);

            //Assert
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedOutput, actualOutput);
        }

        [Fact]
        public void SecondsSinceEpoch_ShouldReturnProperValue()
        {
            //Arrange
            var inputDate = new DateTime(2019, 1, 1, 12, 30, 15);
            ulong expectedResult = 1546345815; //Validated from: https://www.epochconverter.com/

            //Act
            var actualResult = _timeProvider.SecondsSinceEpoch(inputDate);

            //Assert
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public void ConvertTimeFromUtc_ShouldThrowException_WhenUnknownTimezoneTarget()
        {
            //Arrange
            var startDate = DateTime.Now;
            var targetTimezone = "Happy Place";

            //Act.Assert
            Assert.Throws<TimeZoneNotFoundException>(() => _timeProvider.ConvertTimeFromUtc(targetTimezone, startDate));
        }

        [Fact]
        public void ConvertTimeToUtc_ShouldThrowException_WhenUnknownTimezoneTarget()
        {
            //Arrange
            var startDate = DateTime.Now;
            var targetTimezone = "Happy Place";

            //Act.Assert
            Assert.Throws<TimeZoneNotFoundException>(() => _timeProvider.ConvertTimeToUtc(targetTimezone, startDate));
        }
    }
}