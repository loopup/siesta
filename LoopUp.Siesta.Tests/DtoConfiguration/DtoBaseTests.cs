namespace LoopUp.Siesta.Tests.DtoConfiguration
{
    using FluentAssertions;
    using LoopUp.Siesta.DtoConfiguration;
    using LoopUp.Siesta.Exceptions;
    using Xunit;

    public class DtoBaseTests
    {
        [Fact]
        public void GetSingleEndpoint_NotOverridden_ReturnsNull()
        {
            var dto = new DtoBaseTestDto();

            dto.GetSingleEndpoint.Should().BeNull();
        }

        [Fact]
        public void GetManyEndpoint_NotOverridden_ThrowsEndpointNotImplemented()
        {
            var dto = new DtoBaseTestDto();

            dto.GetManyEndpoint.Should().BeNull();
        }

        [Fact]
        public void CreateEndpoint_NotOverridden_ThrowsEndpointNotImplemented()
        {
            var dto = new DtoBaseTestDto();

            dto.CreateEndpoint.Should().BeNull();
        }

        [Fact]
        public void PutEndpoint_NotOverridden_ThrowsEndpointNotImplemented()
        {
            var dto = new DtoBaseTestDto();

            dto.PutEndpoint.Should().BeNull();
        }

        [Fact]
        public void PatchEndpoint_NotOverridden_ThrowsEndpointNotImplemented()
        {
            var dto = new DtoBaseTestDto();

            dto.PatchEndpoint.Should().BeNull();
        }

        [Fact]
        public void DeleteEndpoint_NotOverridden_ThrowsEndpointNotImplemented()
        {
            var dto = new DtoBaseTestDto();

            dto.DeleteEndpoint.Should().BeNull();
        }
    }

    public class DtoBaseTestDto : DtoBase
    {
    }
}
