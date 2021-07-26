namespace LoopUp.Siesta.Tests
{
    using System;
    using LoopUp.Siesta.DtoConfiguration;

    public class TestDto : DtoBase
    {
        public Guid? Id { get; set; }

        public string? StringId { get; set; }

        public int? IntId { get; set; }

        public override string GetSingleEndpoint => "/v1/test";

        public override string GetManyEndpoint => "/v1/test";
    }
}
