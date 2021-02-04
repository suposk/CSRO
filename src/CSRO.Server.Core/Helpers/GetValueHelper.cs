using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Core.Helpers
{
    public static class GetValueHelper 
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

        public static string GetVal(string text, string patern)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(patern))
                return null;

            try
            {
                string value = null;
                List<string> keyValuePairs = text.Split('/').ToList();

                int idx = 0;
                foreach (var keyValuePair in keyValuePairs)
                {
                    idx++;
                    //string key = keyValuePair.Split('/')[0].Trim();
                    string key = keyValuePair.Trim();
                    if (key == patern)
                    {
                        //value = keyValuePair.Split('/')[1];
                        value = keyValuePairs[idx];
                        return value;
                    }
                }

                //value = text
                //    .Split(',')
                //    .Select(
                //        pair => pair.Split('/'))
                //    .ToDictionary(
                //        keyValue => keyValue[0].Trim(),
                //        keyValue => keyValue[1].Trim())
                //    [patern];

                //return value;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}
