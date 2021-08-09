namespace LoopUp.Siesta.Tests.Extensions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Http;
    using FluentAssertions;
    using LoopUp.Siesta.Extensions;
    using Xunit;

    public class TypeExtensionsTests
    {
        #region IsComplexObject

        [Fact]
        public void IsComplexObject_SystemPrimitiveSimpleTypesOrCollections_ReturnFalse()
        {
            List<object> simpleTypes = new List<object>
            {
                1,
                "string",
                0.03M,
                DateTime.UtcNow,
                true,
                new List<int>
                {
                    1,
                    2,
                    10,
                },
                new string[2],
                new List<HttpResponseMessage>
                {
                    new HttpResponseMessage(),
                },
            };

            foreach (var simpleType in simpleTypes)
            {
                simpleType.GetType().IsComplexObject().Should().BeFalse();
            }
        }

        [Fact]
        public void IsComplexObject_AnyClass_ReturnsTrue()
        {
            List<object> complexTypes = new List<object>
            {
                new HttpRequestException(),
                new EnumerableFilterInformation(),
            };

            foreach (var simpleType in complexTypes)
            {
                simpleType.GetType().IsComplexObject().Should().BeTrue();
            }
        }

        #endregion

        #region IsListOfT

        [Fact]
        public void IsListOfT_AnyClassImplementingICollectionOfT_ReturnsTrue()
        {
            List<IEnumerable> objects = new List<IEnumerable>
            {
                new List<int>(),
                new int[2],
                new TestList(),
            };

            foreach (var enumerable in objects)
            {
                enumerable.GetType().IsAListOfT().Should().BeTrue();
            }
        }

        [Fact]
        public void IsListOfT_TypeIsString_ReturnsFalse()
        {
            string anAnnoyingStringThatForSomeReasonIsAnIEnumerableChar = "I am a string and I suck";

            anAnnoyingStringThatForSomeReasonIsAnIEnumerableChar.GetType().IsAListOfT().Should().BeFalse();
        }

        [Fact]
        public void IsListOfT_AnyClassNotImplementingIEnumerable_ReturnsFalse()
        {
            List<object> objects = new List<object>
            {
                new HttpRequestException(),
                default(DateTime),
            };

            foreach (var enumerable in objects)
            {
                enumerable.GetType().IsAListOfT().Should().BeFalse();
            }
        }

        #endregion
    }

    internal class TestList : IList<string>
    {
        public int Count { get; }

        public bool IsReadOnly { get; }

        public string this[int index]
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public IEnumerator<string> GetEnumerator()
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void Add(string item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Contains(string item)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            throw new NotSupportedException();
        }

        public bool Remove(string item)
        {
            throw new NotSupportedException();
        }

        public int IndexOf(string item)
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, string item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
    }
}
