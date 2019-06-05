using Models;
using System.Collections.Generic;

namespace DataAccess.Handlers
{
  public interface IFileHandler
  {
    IEnumerable<Person> ReadFile();
    void WriteFile(IEnumerable<Person> persons);
  }
}
