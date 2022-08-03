namespace Siesta.Configuration.Tests.Serialization
{
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Newtonsoft.Json;
    using Siesta.Configuration.Serialization;
    using Xunit;

    public class SerializablePagedListTests
    {
        [Fact]
        public void SerializablePagedList_WithIEnumerableInput_IsSerializedAndDeserializedProperly()
        {
            IEnumerable<string> sequence = new string[] { "abc", "def", "efg", "ghi", "jkl" };

            var paged = new SerializablePagedList<string>(sequence, 1, 5, sequence.Count());

            JsonConvert.DeserializeObject<DeserializedPagedList<string>>(JsonConvert.SerializeObject(paged))
                .Should().Equal(paged);
        }

        [Fact]
        public void SerializablePagedList_WithIEnumerableInput_IsSerializedAndDeserializedProperly_ForSubseqentPages()
        {
            IEnumerable<string> sequence = new string[] { "abc", "def", "efg", "ghi", "jkl" };

            var pagedToSecond = new SerializablePagedList<string>(sequence, 2, 2, sequence.Count());
            var pagedToThird = new SerializablePagedList<string>(sequence, 3, 2, sequence.Count());

            JsonConvert.DeserializeObject<DeserializedPagedList<string>>(JsonConvert.SerializeObject(pagedToSecond))
                .Should().Equal(pagedToSecond);
            JsonConvert.DeserializeObject<DeserializedPagedList<string>>(JsonConvert.SerializeObject(pagedToThird))
                .Should().Equal(pagedToThird);
        }

        [Fact]
        public void SerializablePagedList_WithIEnumerableInput_IsSerializedAndDeserializedProperly_WhenSetIsEmpty()
        {
            IEnumerable<string> sequence = new string[] { "abc", "def", "efg", "ghi", "jkl" };

            var paged = new SerializablePagedList<string>(sequence, 2, 5, sequence.Count());

            JsonConvert.DeserializeObject<DeserializedPagedList<string>>(JsonConvert.SerializeObject(paged))
                .Should().Equal(paged);
        }
    }
}
