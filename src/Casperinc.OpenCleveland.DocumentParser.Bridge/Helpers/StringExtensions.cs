using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Casperinc.OpenCleveland.DocumentParser.Bridge.Helpers
{
    public static class StringExtensions
    {
        public static string GetSHA256Hash(this string text)
        {
            using(var hash = new SHA256Managed())
            {
                var hashValue = String.Concat(
                    hash.ComputeHash(Encoding.UTF8.GetBytes(text))
                        .Select(item => item.ToString("x2"))
                );
                return hashValue;
            }
        }
    }
}