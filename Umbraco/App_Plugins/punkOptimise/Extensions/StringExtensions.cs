using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace punkOptimise.Extensions
{
    public static class StringExtensions
    {
        public static IList<string> SplitToList(this string input)
        {
            return input.Split(',')
                .Select(f => f.Trim())
                .Where(f => !string.IsNullOrEmpty(f))
                .ToList();
        }
    }
}