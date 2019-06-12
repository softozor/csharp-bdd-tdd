using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PersonManagementModule.ViewModels
{
  public class PersonViewModel : BindableBase
  {
    public ObservableCollection<PersonItem> Persons { get; set; }
    public DelegateCommand SavePersonsCommand { get; set; }

    public PersonViewModel()
    {
    }
  }
}
