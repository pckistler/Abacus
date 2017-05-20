using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abacus.Utility
{
    //public class MatchFactory<Item, Property>
    //{
    //    IGetTheValueOfAProperty<Item, Property> accessor;

    //    public MatchFactory(IGetTheValueOfAProperty<Item, Property> accessor)
    //    {
    //        this.accessor = accessor;
    //    }

    //    public Criteria<Item> equal_to(Property value)
    //    {
    //        return x => accessor(x).Equals(value);
    //    }

    //    public Criteria<Item> equal_to_any(params Property[] values)
    //    {
    //        return values.reduce(Criterias.never_matches<Item>(),
    //          (accumulator, value) => accumulator.or(equal_to(value)));
    //    }

    //    public Criteria<Item> not_equal_to(Property value)
    //    {
    //        return equal_to(value).not();
    //    }
    //}

    class MyStringCompare : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return string.Compare(x, y, true) == 0;
        }
        public int GetHashCode(string s)
        {
            return s.GetHashCode();
        }
    }
}