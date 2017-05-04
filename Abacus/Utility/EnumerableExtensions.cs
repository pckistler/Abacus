using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abacus.Utility
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<Item> one_at_a_time<Item>(this IEnumerable<Item> items)
        {
            foreach (var item in items) yield return item;
        }

        //public static IEnumerable<Output> map<Input, Output>(this IEnumerable<Input> items, IMapFrom<Input, Output> mapper)
        //{
        //    foreach (var item in items) yield return mapper(item);
        //}

        //public static Output reduce<Input, Output>(this IEnumerable<Input> items, Output initial_value, IReduce<Input, Output> reducer)
        //{
        //    var current_value = initial_value;

        //    foreach (var item in items)
        //        current_value = reducer(current_value, item);

        //    return current_value;
        //}

        public static IEnumerable<Item> all_items_matching<Item>(this IEnumerable<Item> items, Criteria<Item> criteria)
        {
            foreach (var m in items)
                if (criteria(m))
                    yield return m;
        }

        //public static IEnumerable<Item> all_items_matching<Item>(this IEnumerable<Item> items, Criteria<Item> criteria)
        //{
        //    return items.all_items_matching(criteria.matches);
        //}
    }
}