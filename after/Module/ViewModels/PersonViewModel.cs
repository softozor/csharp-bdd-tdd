using DataAccess.Handlers;
using PersonManagementModule.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonManagementModule.ViewModels
{
  public class PersonViewModel : BindableBase
  {
    readonly IFileHandlerFactory _fileHandlerFactory;
    readonly IPersonProvider _personProvider;

    public ObservableCollection<PersonItem> Persons { get; set; }

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
