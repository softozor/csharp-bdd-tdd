using System;

namespace DataAccess.Handlers
{
  public class UnsupportedFileTypeException : Exception
  {
    public UnsupportedFileTypeException(string str) : base(str)
    {
    }
  }
}
