using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class LinqHelpersBE
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
