
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MidiBard.HSC
{

    public static class Extensions
    {

        public static Dictionary<TKey,List<TVal>> DictionaryGroupBy<TVal, TKey>(
            this IEnumerable<TVal> items, 
            Func<TVal, TKey> selector,
            Func<KeyValuePair<TKey, List<TVal>>, bool> cond = null)
        {
            var groups = new Dictionary<TKey, List<TVal>>();

            foreach (var item in items)
            {
                if (!groups.ContainsKey(selector(item)))
                    groups.Add(selector(item), new List<TVal>() { item });
                else
                    groups[selector(item)].Add(item);
            }

            if (cond == null)
                return groups;

            return groups.AsParallel().Where(g => cond(g)).ToDictionary(g => g.Key, g => g.Value);

        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        {
            var groups = source.GroupBy(selector).Select(grp => new { Key = grp.Key, Value = grp });
            return groups.Select(x => x.Value.First());
        }

        public static int ToInt(this bool boolVal)
        {
            return boolVal ? 1 : 0;
        }

        public static bool ToBool(this int intVal)
        {
            return intVal >= 1;
        }

        public static string JoinWithSpaces(this string[] args)
        {
            return args.IsNullOrEmpty() ? null : String.Join(" ", args);
        }

        public static string ToCsv(this IEnumerable<int> nums)
        {
            return nums.Select(n => n.ToString()).Aggregate((x, y) => x + "," + y);
        }

        public static bool IsMidiFile(this string filePath)
        {
            return Path.GetExtension(filePath) == "mid" || Path.GetExtension(filePath) == "midi";
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> items)
        {
            return items == null || !items.Any();
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T ToEnum<T>(this int value)
        {
            var name = Enum.GetName(typeof(T), value);
            return name.ToEnum<T>();
        }

        public static void AddIfNotKeyExists<TKey, TVal>(this Dictionary<TKey, TVal> src, TKey key, TVal val)
        {
            if (!src.ContainsKey(key))
            {
                src.Add(key, val);
            }
        }

        public static void AddOrUpdate<TKey, TVal>(this Dictionary<TKey, TVal> src, TKey key, TVal val)
        {
            if (!src.ContainsKey(key))
            {
                src.Add(key, val);
            }
            else
            {
                src[key] = val;
            }
        }

        public static string CommaSeparated<T>(this IEnumerable<T> values)
        {
            return values.Select(v => v.ToString()).Aggregate((x, y) => x + "," + y);
        }

    
    }
}
