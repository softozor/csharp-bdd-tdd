using System;
using System.Collections.Generic;
using DataAccess;
using Models;

namespace PersonManagementModule.Services
{
  public class PersonProvider : IPersonProvider
  {
    readonly IDataService _dataService;

    public PersonProvider(IDataService dataService)
    {
      _dataService = dataService;
    }

    public string Filename { get; private set; }
    public FileFormat FileType { get; private set; }

    public void Save(IEnumerable<Person> persons)
    {
      _dataService.SavePersons(persons ?? throw new NullReferenceException("Cannot save null list of persons"));
    }

    public IEnumerable<Person> GetPersons()
    {
      return _dataService.GetAllPersons();
    }
  }
}
