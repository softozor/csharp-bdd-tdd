using Models;
using PersonManagementModule.ViewModels;
using TestUtils;

namespace PersonManagementSpec
{
  /// <summary>The PersonManager class is the domain-specific framework for our scenarios. It is a helper 
  /// class encapsulating some of the necessary person management module manipulations.
  /// </summary>
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
      var model = _personGenerator.Generate();
      var personItem = new PersonItem(model);
      _viewModel.Persons.Add(personItem);
      return model;
    }

    public void Save()
    {
      _viewModel.SavePersonsCommand.Execute();
    }
  }
}
