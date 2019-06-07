using Prism.Mvvm;

namespace Module.ViewModels
{
  public class PersonViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public PersonViewModel()
        {
            Message = "Person View";
        }
    }
}
