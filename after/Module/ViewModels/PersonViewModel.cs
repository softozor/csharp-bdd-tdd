using PersonManagementModule.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonManagementModule.ViewModels
{
  public class PersonViewModel : BindableBase
  {
    readonly IPersonProvider _personProvider;

    public ObservableCollection<PersonItem> Persons { get; set; }

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
  }
}
