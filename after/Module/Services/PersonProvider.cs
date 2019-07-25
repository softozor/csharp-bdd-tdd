using DataAccess.Services;
using PersonManagementModule.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PersonManagementModule.Services
{
  public class PersonProvider : IPersonProvider
  {
    readonly IDataService _dataService;

    public PersonProvider(IDataService dataService)
    {
      _dataService = dataService;
    }

    public void Save(IEnumerable<PersonItem> persons)
    {
      if (persons == null)
      {
        throw new NullReferenceException("Cannot save null list of persons");
      }
      var models = from item in persons select item.Model;
      _dataService.SavePersons(models);
    }

    public IEnumerable<PersonItem> GetPersons()
    {
      var persons = _dataService.GetAllPersons();
      return from model in persons select new PersonItem(model);

    }
  }
}
