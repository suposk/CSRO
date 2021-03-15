﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class LinqHelpersUI
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
            return source == null || source.Any() == false;
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
            return source != null && source.Any();
        }
    }
}
