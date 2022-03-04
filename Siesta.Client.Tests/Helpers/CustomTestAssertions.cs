namespace Siesta.Client.Tests.Helpers
{
    using System;
    using FluentAssertions;

    public static class CustomTestAssertions
    {
        public static void ShouldBeEquivalentToThrownException<T>(this T exception, T expectedException)
            where T : Exception =>
            exception.Should().BeEquivalentTo(expectedException, options => options
                .Excluding(r => r.TargetSite)
                .Excluding(r => r.StackTrace)
                .Excluding(r => r.Source));
    }
}
