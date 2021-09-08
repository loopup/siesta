namespace Siesta.Configuration.Patch
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.AspNetCore.JsonPatch;
    using Siesta.Configuration.Extensions;

    /// <summary>
    /// Helper class for making and working with JsonPatchDocuments.
    /// </summary>
    public static class JsonPatchDocumentHelpers
    {
        /// <summary>
        /// Generate a JsonPatchDocument through the comparison of two versions of the same class.
        /// </summary>
        /// <param name="original">Original version of the class.</param>
        /// <param name="modified">New version of the class.</param>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <returns>A JsonPatchDocument for the two objects.</returns>
        public static JsonPatchDocument GeneratePatchDocument<T>(T original, T modified)
            where T : class
        {
            var patchDocument = new JsonPatchDocument();

            PopulatePatchForObject(original, modified, patchDocument, string.Empty);

            return patchDocument;
        }

        /// <summary>
        /// Recursive method to populate the Patch document for 2 objects.
        /// </summary>
        /// <param name="originalObject">The original version of the object.</param>
        /// <param name="modifiedObject">The modified version of the object.</param>
        /// <param name="patchDocument">The PATCH document to populate.</param>
        /// <param name="path">The path to use for PATCH document paths.</param>
        /// <typeparam name="T">The object type.</typeparam>
        private static void PopulatePatchForObject<T>(T? originalObject, T? modifiedObject, JsonPatchDocument patchDocument, string path)
            where T : class
        {
            // Use whichever object is no null to get the properties on the object
            // If both are null then we create an empty list
            var nonNullObject = originalObject ?? modifiedObject;
            var properties = nonNullObject is null ? new List<PropertyInfo>() : nonNullObject.GetType()
                .GetProperties(BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public).ToList();

            // Now we loop through each property and try and create the PATCH document entry for it
            foreach (var property in properties)
            {
                // Get the values of each property
                var originalValue = originalObject != null ? property.GetValue(originalObject) : null;
                var modifiedValue = modifiedObject != null ? property.GetValue(modifiedObject) : null;

                if (originalValue is null && modifiedValue is not null)
                {
                    // Simplest case where the original was null and new isn't, we just add the new value
                    patchDocument.Add($"{path}/{property.Name}", modifiedValue);
                }
                else if (originalValue is not null && modifiedValue is null)
                {
                    // Reverse of the above, we just remove
                    patchDocument.Remove($"{path}/{property.Name}");
                }
                else if (property.PropertyType.IsComplexObject())
                {
                    // First complicated part, the IsComplexObject should reflect whether the current property
                    // Is something that we want to step into to create the PATCH
                    // E.g this could be a nested class, like an Address on a User
                    // To do this we just recursively calls this method, adjusting the path to reflect going a level deeper
                    PopulatePatchForObject(
                        originalObject != null ? property.GetValue(originalObject) : null,
                        modifiedObject != null ? property.GetValue(modifiedObject) : null,
                        patchDocument,
                        $"{path}/{property.Name}");
                }
                else if (property.PropertyType.IsAListOfT())
                {
                    // Second complicated situation
                    // Here the property is a List
                    // In this situation we don't want to replace the whole list so we use this special method
                    PopulatePatchForList(
                        originalObject != null ? (IList<object>)property.GetValue(originalObject) ! : null,
                        modifiedObject != null ? (IList<object>)property.GetValue(modifiedObject) ! : null,
                        patchDocument,
                        $"{path}/{property.Name}");
                }
                else
                {
                    if (originalValue != modifiedValue)
                    {
                        // If the properties are not complex or lists we just compare them to see if they are different
                        if (modifiedValue is null || string.IsNullOrWhiteSpace(modifiedValue.ToString()))
                        {
                            // Remove the value when it becomes null
                            patchDocument.Remove($"{path}/{property.Name}");
                        }
                        else if (originalValue is null || string.IsNullOrWhiteSpace(originalValue.ToString()))
                        {
                            // Add the value if the value was null
                            patchDocument.Add($"{path}/{property.Name}", modifiedValue);
                        }
                        else
                        {
                            // Replace the value if the value was not null and is still not null but different
                            patchDocument.Replace($"{path}/{property.Name}", modifiedValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Populate the patch document for a List.
        /// </summary>
        /// <param name="originalList">The original list.</param>
        /// <param name="modifiedList">The new version of the list.</param>
        /// <param name="patchDocument">The patch document to populate.</param>
        /// <param name="path">The path to use as the base for these objects.</param>
        /// <typeparam name="T">The type of the List.</typeparam>
        private static void PopulatePatchForList<T>(
            IList<T>? originalList,
            IList<T>? modifiedList,
            JsonPatchDocument patchDocument,
            string path)
        {
            // When both are not null we have to do some complicated comparisons
            if (originalList is not null && modifiedList is not null)
            {
                // First we an check if the lists are the exact same, if they are we do nothing
                if (!originalList.SequenceEqual(modifiedList))
                {
                    // First we compare the original and the modified and work out what needs to be removed from the original
                    // We need to keep track of what is removed and where from for later indexs
                    var itemsToRemove = new List<(T item, int indexInOriginal)>();
                    foreach (var item in originalList)
                    {
                        var indexInModified = modifiedList.IndexOf(item);
                        var indexInOriginal = originalList.IndexOf(item);

                        if (indexInModified < 0)
                        {
                            // We remove at the index it is at in the original minus any other removes that have happened
                            // This is because as we remove items the later indexes are lowered
                            patchDocument.Remove($"{path}/{indexInOriginal - itemsToRemove.Count}");
                            itemsToRemove.Add((item, indexInOriginal));
                        }
                    }

                    // Now we compare again but from the modified point of view to see what we need to add and move
                    // Again we have to keep track of this for indexes
                    var itemsToAdd = new List<(T item, int indexInModified)>();
                    foreach (var item in modifiedList)
                    {
                        var indexInOriginal = originalList.IndexOf(item);
                        var indexInModified = modifiedList.IndexOf(item);

                        if (indexInOriginal < 0)
                        {
                            // Here the item has been added
                            // If it is at the end we use a different syntax "-"
                            // We can just add at the specified index
                            patchDocument.Add(
                                indexInModified == modifiedList.Count - 1 ? $"{path}/-" : $"{path}/{indexInModified}", item);
                            itemsToAdd.Add((item, indexInModified));
                        }
                        else if (indexInOriginal != indexInModified)
                        {
                            // Here an item has moved
                            // The index is complicated
                            // It is moved from the original minus any was have removed before it + any we add before it
                            // It simpy moves to the place we want it to end up
                            var indexToMoveFrom = indexInOriginal -
                                                  itemsToRemove.Count(i => i.indexInOriginal < indexInOriginal) +
                                                  itemsToAdd.Count(i => i.indexInModified <= indexInOriginal);
                            patchDocument.Move($"{path}/{indexToMoveFrom}", $"{path}/{indexInModified}");
                        }
                    }
                }
            }
            else if (originalList is null && modifiedList is not null)
            {
                // Easy if list was null and is now not we add the whole list
                patchDocument.Add(path, modifiedList);
            }
            else if (originalList is not null)
            {
                // Again easy we have remvoed the list and therefore we just remove the whole thing
                patchDocument.Remove(path);
            }
        }
    }
}
