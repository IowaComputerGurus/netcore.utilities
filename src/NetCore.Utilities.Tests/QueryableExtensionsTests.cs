using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ICG.NetCore.Utilities.Tests;

/// <summary>
/// Unit tests for <see cref="QueryableExtensions"/>.
/// </summary>
public class QueryableExtensionsTests
{
    private static IQueryable<TestEntity> GetTestEntities()
    {
        return new List<TestEntity>
        {
            new TestEntity { Id = 1, Name = "Alpha" },
            new TestEntity { Id = 2, Name = "Beta" },
            new TestEntity { Id = 3, Name = "Gamma" },
            new TestEntity { Id = 4, Name = "Alpha" }
        }.AsQueryable();
    }

    private class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    [Fact]
    public void WhereIf_ConditionTrue_FiltersResults()
    {
        var query = GetTestEntities();
        var result = query.WhereIf(true, x => x.Name == "Alpha").ToList();

        Assert.Equal(2, result.Count);
        Assert.All(result, x => Assert.Equal("Alpha", x.Name));
    }

    [Fact]
    public void WhereIf_ConditionFalse_ReturnsOriginal()
    {
        var query = GetTestEntities();
        var result = query.WhereIf(false, x => x.Name == "Alpha").ToList();

        Assert.Equal(4, result.Count);
    }

    [Fact]
    public void OrderByIf_ConditionTrue_OrdersResults()
    {
        var query = GetTestEntities();
        var result = query.OrderByIf(true, x => x.Name).ToList();
        string[] expectedOrder = ["Alpha", "Alpha", "Beta", "Gamma"];

        Assert.Equal(expectedOrder, result.Select(x => x.Name).ToArray());
    }

    [Fact]
    public void OrderByIf_ConditionFalse_ReturnsOriginal()
    {
        var query = GetTestEntities();
        var result = query.OrderByIf(false, x => x.Name).ToList();
        int[] expectedOrder = [1, 2, 3, 4];

        Assert.Equal(expectedOrder, result.Select(x => x.Id).ToArray());
    }

    [Fact]
    public void OrderByDescendingIf_ConditionTrue_OrdersDescending()
    {
        var query = GetTestEntities();
        var result = query.OrderByDescendingIf(true, x => x.Name).ToList();
        string[] expectedOrder = ["Gamma", "Beta", "Alpha", "Alpha"];

        Assert.Equal(expectedOrder, result.Select(x => x.Name).ToArray());
    }

    [Fact]
    public void OrderByDescendingIf_ConditionFalse_ReturnsOriginal()
    {
        var query = GetTestEntities();
        var result = query.OrderByDescendingIf(false, x => x.Name).ToList();
        int[] expectedOrder = [1, 2, 3, 4];

        Assert.Equal(expectedOrder, result.Select(x => x.Id).ToArray());
    }

    [Theory]
    [InlineData(1, 2, new[] { 1, 2 })]
    [InlineData(2, 2, new[] { 3, 4 })]
    public void GetPage_ReturnsCorrectPage(int pageNumber, int pageSize, int[] expectedIds)
    {
        var query = GetTestEntities();
        var result = query.GetPage(pageNumber, pageSize).ToList();

        Assert.Equal(expectedIds, result.Select(x => x.Id).ToArray());
    }

    [Fact]
    public void DistinctBy_ReturnsDistinctElements()
    {
        var query = GetTestEntities();
        var result = query.DistinctBy(x => x.Name).ToList();

        var distinctNames = result.Select(x => x.Name).ToArray();
        Assert.Equal(3, distinctNames.Length);
        Assert.Contains("Alpha", distinctNames);
        Assert.Contains("Beta", distinctNames);
        Assert.Contains("Gamma", distinctNames);
    }
}