using System;
using System.Collections.Generic;
using System.Linq;
using Casperinc.OpenCleveland.DocumentParser.Bridge.Models;
using static Dapper.SqlMapper;

namespace Casperinc.OpenCleveland.DocumentParser.Bridge.Helpers
{
    // https://stackoverflow.com/questions/6379155/multi-mapper-to-create-object-hierarchy
    public static class GridReaderExtensions
    {
        public static IEnumerable<TFirst> Map<TFirst, TSecond, TKey>
        (
            this GridReader reader,
            Func<TFirst, TKey> firstKey, 
            Func<TSecond, TKey> secondKey, 
            Action<TFirst, IEnumerable<TSecond>> addChildren
        )
        {
            var first = reader.Read<TFirst>().ToList();
            return first;
        }


    }
}