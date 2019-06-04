using System;

namespace DataAccess
{
  public class UnsupportedFileTypeException : Exception
  {
    public UnsupportedFileTypeException(string str) : base(str)
    {
    }
  }
}
