namespace LoopUp.Siesta.Tests.Patch
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using FluentAssertions;
    using LoopUp.Siesta.Patch;
    using Xunit;

    public class JsonPatchDocumentHelpersTests
    {
        #region GeneratePatchDocument

        #region Top level values

        [Fact]
        public void GeneratePatchDocument_TopLevelValueChangedToNull_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                GuidProperty = Guid.NewGuid(),
            };
            var modified = new TestClass
            {
                GuidProperty = null,
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_TopLevelValueChangedToNonNullFromNull_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                GuidProperty = null,
            };
            var modified = new TestClass
            {
                GuidProperty = Guid.NewGuid(),
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_TopLevelValueChanged_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                GuidProperty = Guid.NewGuid(),
            };
            var modified = new TestClass
            {
                GuidProperty = Guid.NewGuid(),
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        #endregion

        #region Nested class

        [Fact]
        public void GeneratePatchDocument_NestedPropertyChangedToNullFromNonNull_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                NestedProperty = new NestedProperty
                {
                    NestedInt = 10,
                },
            };
            var modified = new TestClass
            {
                NestedProperty = new NestedProperty
                {
                    NestedInt = null,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_NestedPropertyChangedFromNullToNonNull_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                NestedProperty = new NestedProperty
                {
                    NestedInt = null,
                },
            };
            var modified = new TestClass
            {
                NestedProperty = new NestedProperty
                {
                    NestedInt = 10,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_NestedPropertyChangedFromNonNullToNonNull_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                NestedProperty = new NestedProperty
                {
                    NestedInt = 12,
                },
            };
            var modified = new TestClass
            {
                NestedProperty = new NestedProperty
                {
                    NestedInt = 10,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_NestedObjectChangedFromNullToNonNull_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                NestedProperty = null,
            };
            var modified = new TestClass
            {
                NestedProperty = new NestedProperty
                {
                    NestedInt = 10,
                    NestedString = "string thing",
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_NestedObjectChangedFromNonNullToNull_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                NestedProperty = new NestedProperty
                {
                    NestedInt = 10,
                    NestedString = "string thing",
                },
            };
            var modified = new TestClass
            {
                NestedProperty = null,
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsBuiltInComplexType_CreatesCorrectPatchDocument()
        {
            var original = new DateTimeClass
            {
                DateTime = new DateTime(2021, 1, 1),
            };
            var modified = new DateTimeClass
            {
                DateTime = new DateTime(2021, 1, 2),
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        #endregion

        #region Collections

        [Fact]
        public void GeneratePatchDocument_PropertyIsListWithTheSameOrder_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    10,
                },
            };
            var modified = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    10,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsListWithChangedOrder_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    10,
                },
            };
            var modified = new TestClass
            {
                ListObject = new List<object>
                {
                    2,
                    10,
                    1,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsListItemInOriginalButNotModified_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    10,
                },
            };
            var modified = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    10,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsListItemInModifiedButNotOriginal_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                },
            };
            var modified = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    10,
                    2,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsListItemAddedToEnd_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                },
            };
            var modified = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    10,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsListMultipleOperations_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    3,
                    4,
                    5,
                    6,
                    7,
                    8,
                    9,
                    10,
                    11,
                    12,
                    13,
                    14,
                    15,
                },
            };
            var modified = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    17,
                    7,
                    8,
                    6,
                    9,
                    11,
                    10,
                    20,
                    14,
                    13,
                    18,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsListOriginalNullModifiedNot_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = null,
            };
            var modified = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    10,
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsListModifiedNullOriginalNot_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = new List<object>
                {
                    1,
                    2,
                    10,
                },
            };
            var modified = new TestClass
            {
                ListObject = null,
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        [Fact]
        public void GeneratePatchDocument_PropertyIsListOfComplexObjects_CreatesCorrectPatchDocument()
        {
            var original = new TestClass
            {
                ListObject = new List<object>
                {
                    new HttpRequestMessage(),
                    new HttpRequestMessage(),
                    new HttpRequestMessage(),
                },
            };
            var modified = new TestClass
            {
                ListObject = new List<object>
                {
                    new HttpRequestMessage(),
                    new HttpRequestMessage(),
                },
            };

            var patchDocument = JsonPatchDocumentHelpers.GeneratePatchDocument(original, modified);

            patchDocument.ApplyTo(original);
            original.Should().BeEquivalentTo(modified);
        }

        #endregion

        #endregion
    }

    public class TestClass
    {
        public Guid? GuidProperty { get; set; }

        public NestedProperty? NestedProperty { get; set; }

        public List<NestedProperty>? ListNestedProperty { get; set; }

        public List<object>? ListObject { get; set; }
    }

    public class NestedProperty
    {
        public int? NestedInt { get; set; }

        public string? NestedString { get; set; }

        public object? NestedObject { get; set; }
    }

    public class DateTimeClass
    {
        public DateTime DateTime { get; set; }
    }
}
