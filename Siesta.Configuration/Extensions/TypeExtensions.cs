namespace Siesta.Configuration.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Extension method for <see cref="Type"/> class.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        /// Method to determine if the Type is a complex type, that is something other than a primitive, built in system type or enumerable.
        /// We can think of this as anything that is a class with properties.
        /// </summary>
        /// <param name="type">The type to test for complexity.</param>
        /// <returns>Boolean reflecting type complexity.</returns>
        public static bool IsComplexObject(this Type type)
        {
            if (type.IsPrimitive || type == typeof(string) || IsAListOfT(type))
            {
                return false;
            }

            if (type.IsClass)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Used to determine if a type is an enumerable.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>Boolean reflecting if type enumerable.</returns>
        public static bool IsAListOfT(this Type type)
        {
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)))
            {
                return true;
            }

            return false;
        }
    }
}
