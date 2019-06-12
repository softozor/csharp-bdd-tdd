using DataAccess.Services;
using Models;
using System;
using System.Collections.Generic;

namespace PersonManagementModule.Services
{
  public class PersonProvider : IPersonProvider
  {
    readonly IDataService _dataService;

    public PersonProvider(IDataService dataService)
    {
      _dataService = dataService;
    }

    public void Save(IEnumerable<Person> persons)
    {
      _dataService.SavePersons(persons ?? throw new NullReferenceException("Cannot save null list of persons"));
    }

    public IEnumerable<Person> GetPersons()
    {
      throw new NotImplementedException();
    }
  }
}
