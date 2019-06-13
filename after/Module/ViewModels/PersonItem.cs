using Models;
using System;

namespace PersonManagementModule.ViewModels
{
  public class PersonItem
  {
    public Person Model { get; set; }

    public uint Id { get => Model.Id; set => Model.Id = value; }

    public string FirstName { get => Model.FirstName; set => Model.FirstName = value; }

    public string LastName { get => Model.LastName; set => Model.LastName = value; }

    public string Title { get => Model.Title; set => Model.Title = value; }

    public PersonItem(Person model)
    {
      Model = model ?? throw new NullReferenceException("Wrapped person model cannot be null.");
    }
  }
}
