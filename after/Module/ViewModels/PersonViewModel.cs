using PersonManagementModule.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonManagementModule.ViewModels
{
  /// <summary>
  /// The PersonViewModel is the central view model for person management.
  /// </summary>
  public class PersonViewModel : BindableBase
  {
    readonly IPersonProvider _personProvider;

    /// <summary>Gets or sets the observable list of persons that can be linked with a view.</summary>
    /// <value>The persons.</value>
    public ObservableCollection<PersonItem> Persons { get; set; }


    /// <summary>Initializes a new instance of the <see cref="PersonViewModel"/> class with persons gathered from the <paramref name="personProvider"/>.</summary>
    /// <param name="personProvider">The person provider <see cref="IPersonProvider"/>.
    /// If it is null, the DI container throws a runtime exception.</param>
    public PersonViewModel(IPersonProvider personProvider)
    {
      _personProvider = personProvider;

      LoadPersons();

      SavePersonsCommand = new DelegateCommand(SavePersons, CanSavePersons);
    }

    private void LoadPersons()
    {
      var personItemList = from model in _personProvider.GetPersons() select new PersonItem(model);
      Persons = new ObservableCollection<PersonItem>(personItemList);
    }

    /// <summary>  Command to be called e.g. by a view in order to persist the persons listed in the <see cref="Persons"/> collection to the database.</summary>
    /// <value>The save persons command.</value>
    public DelegateCommand SavePersonsCommand { get; set; }

    private void SavePersons()
    {
      var personModels = from item in Persons select item.Model;
      _personProvider.Save(personModels);
    }

    private bool CanSavePersons()
    {
      return true;
    }
  }
}
