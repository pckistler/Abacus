using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Abacus.Utility
{
    public delegate bool Criteria<in Item>(Item item);

}