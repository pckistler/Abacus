using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abacus.Utility.Matching
{
    //public class OrCriteria
    //{
    //    Criteria<Item> left;
    //    Criteria<Item> right;

    //    public OrCriteria(Criteria<Item> left, Criteria<Item> right)
    //    {
    //        this.left = left;
    //        this.right = right;
    //    }

    //    public bool matches(Item item)
    //    {
    //        return left(item) || right(item);
    //    }
    //}
    public static class CriteriaExtensions
    {
        public static Criteria<Item> or<Item>(this Criteria<Item> left,
          Criteria<Item> right)
        {
            return x => left(x) || right(x);
        }

        public static Criteria<Item> not<Item>(this Criteria<Item> left)
        {
            return x => !left(x);
        }
    }
}