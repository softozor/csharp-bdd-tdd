using Models;
using PersonManagementModule.ViewModels;
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
  }
}
