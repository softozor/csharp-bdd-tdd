using System;
using System.Collections.Generic;
using System.Linq;

namespace TestUtils
{
  public static class LinqExtensions
  {
    public static IEnumerable<T> Except<T>(this IEnumerable<T> lhs, IEnumerable<T> rhs, Func<T, T, bool> lambda)
    {
      return lhs.Except(rhs, new LambdaComparer<T>(lambda));
    }
  }
}
