using FizzWare.NBuilder;
using FizzWare.NBuilder.PropertyNaming;
using Models;
using PersonManagementModule.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Spec
{
  public class PersonManager
  {
    readonly PersonViewModel _viewModel;

    public PersonManager(PersonViewModel viewModel)
    {
      _viewModel = viewModel;

      SetupDataBuilder();
    }

    private void SetupDataBuilder()
    {
      BuilderSetup.SetDefaultPropertyName(new RandomValuePropertyNamer(new BuilderSettings()));
    }

    public IEnumerable<Person> GetAccessiblePersons()
    {
      return from personItem in _viewModel.Persons select personItem.Model;
    }

    public Person AddNewPerson()
    {
      var person = Builder<Person>.CreateNew().Build();
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
