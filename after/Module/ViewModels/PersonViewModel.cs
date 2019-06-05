using DataAccess.Handlers;
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
    readonly IFileHandlerFactory _fileHandlerFactory;

    /// <summary>Gets or sets the observable list of persons that can be linked with a view.</summary>
    /// <value>The persons.</value>
    public ObservableCollection<PersonItem> Persons { get; set; }

    /// <summary>Initializes a new instance of the <see cref="PersonViewModel"/> class with persons gathered from the <paramref name="personProvider"/>.</summary>
    /// <param name="personProvider">The person provider <see cref="IPersonProvider"/>.
    /// If it is null, the DI container throws a runtime exception.</param>
    public PersonViewModel(IFileHandlerFactory fileHandlerFactory, IPersonProvider personProvider)
    {
      _fileHandlerFactory = fileHandlerFactory;
      _personProvider = personProvider;

      LoadPersons();

      SavePersonsCommand = new DelegateCommand(SavePersons, CanSavePersons);
      ImportPersonsCommand = new DelegateCommand<ImportPayload>(ImportPersons, CanImportPersons);
    }

    private void LoadPersons()
    {
      var personItemList = from model in _personProvider.GetPersons() select new PersonItem(model);
      Persons = new ObservableCollection<PersonItem>(personItemList);
    }

    #region SavePersons

    /// <summary>Command to be called e.g. by a view in order to persist the persons listed in the <see cref="Persons"/> collection to the database.</summary>
    public DelegateCommand SavePersonsCommand { get; set; }
    /// <summary>Command to be called e.g. by a view in order to import a specified list of persons.</summary>

    private void SavePersons()
    {
      var personModels = from item in Persons select item.Model;
      _personProvider.Save(personModels);
    }

    private bool CanSavePersons()
    {
      return true;
    }

    #endregion

    #region PersonsImportation

    public DelegateCommand<ImportPayload> ImportPersonsCommand { get; set; }

    private void ImportPersons(ImportPayload payload)
    {
      var fileHandler = _fileHandlerFactory.Create(payload.Filename, payload.FileType);
      var persons = fileHandler.ReadFile();
      var newPersonItems = from person in persons select new PersonItem(person);
      Persons.AddRange(newPersonItems);
    }

    private bool CanImportPersons(ImportPayload payload)
    {
      return true;
    }

    #endregion
  }
}
