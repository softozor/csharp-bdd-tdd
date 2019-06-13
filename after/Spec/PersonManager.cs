using Models;
using PersonManagementModule.ViewModels;
using System.Collections.Generic;
using System.Linq;
using TestUtils;

namespace Spec
{
  public class PersonManager
  {
    readonly PersonViewModel _viewModel;
    readonly NewSinglePersonGenerator _personGenerator;

    public PersonManager(NewSinglePersonGenerator personGenerator, PersonViewModel viewModel)
    {
      _personGenerator = personGenerator;
      _viewModel = viewModel;
    }

    public IEnumerable<Person> GetAccessiblePersons()
    {
      return from personItem in _viewModel.Persons select personItem.Model;
    }

    public Person AddNewPerson()
    {
      var person = _personGenerator.Generate();
      var personItem = new PersonItem(person);
      _viewModel.Persons.Add(personItem);
      return person;
    }

    public void Save()
    {
      _viewModel.SavePersonsCommand.Execute();
    }

    public void Import(string filename)
    {
      var payload = new ImportPayload()
      {
        Filename = filename,
        FileType = "JSON"
      };
      _viewModel.ImportPersonsCommand.Execute(payload);
    }
  }
}
