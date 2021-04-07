using System;
using System.Collections.Generic;
using System.Linq;

namespace CSRO.Common.Helpers
{
    public static class LinqHelpersCommon
    {
        /// <summary>
        /// Is Collection null or empty.
        /// Jan's Extension method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyCollection<T>(this IEnumerable<T> source)
        {
            return source == null || source.Any() == false || source.FirstOrDefault() == null;
        }

        /// <summary>
        /// Collection NOT null or NOT empty.
        /// Jan's Extension method.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool HasAnyInCollection<T>(this IEnumerable<T> source)
        {
            return !IsNullOrEmptyCollection(source);
        }
    }
}
