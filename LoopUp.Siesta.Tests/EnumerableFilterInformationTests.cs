namespace LoopUp.Siesta.Tests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using Xunit;

    public class EnumerableFilterInformationTests
    {
        [Fact]
        public void AsQueryDictionary_AddsAllFields()
        {
            var filters = new TestEnumerableFilterInformation
            {
                Name = "My name",
            };

            var expectedResult = new Dictionary<string, string> { { "PageNumber", "1" }, { "PageSize", "25" }, { "Name", "My name" } };

            var result = filters.AsQueryDictionary();

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public void AsQueryDictionary_DoesntIncludeNullFields()
        {
            var filters = new TestEnumerableFilterInformation
            {
                Name = null,
            };

            var expectedResult = new Dictionary<string, string> { { "PageNumber", "1" }, { "PageSize", "25" } };

            var result = filters.AsQueryDictionary();

            result.Should().BeEquivalentTo(expectedResult);
        }
    }

    public class TestEnumerableFilterInformation : EnumerableFilterInformation
    {
        public string? Name { get; set; }
    }
}
