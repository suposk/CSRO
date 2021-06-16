using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//namespace CSRO.Common.Helpers
namespace System
{
    public static class StringHelper
    {
        public static string ReplaceWithStars(this string text, int visibleCharsCount = 8)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= visibleCharsCount)
                return text;
            var result = text.Substring(0, visibleCharsCount);
            var stars = new string('*', text.Length - visibleCharsCount);
            result += stars;
            return result;
        }
    }
}
