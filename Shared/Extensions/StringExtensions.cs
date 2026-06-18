using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Extensions
{
    public static class StringExtensions
    {
        public static long? ToLong(this string? value) =>
            long.TryParse(value, out var result) ? result : null;

        public static int? ToInt(this string? value) =>
            int.TryParse(value, out var result) ? result : null;

        public static string Format(this string template, params object[] args) =>
            args is { Length: > 0 } ? string.Format(template, args) : template;

        public static bool HasValue(this string? value) => !string.IsNullOrWhiteSpace(value);
    }
}
