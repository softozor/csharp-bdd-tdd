using Models;
using System;
using System.Collections.Generic;

namespace DataAccess.Services
{
  public interface IDataService : IDisposable
  {
    void SavePersons(IEnumerable<Person> persons);

    IEnumerable<Person> GetAllPersons();
  }
}
