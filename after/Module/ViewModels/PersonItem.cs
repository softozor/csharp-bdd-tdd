using Models;
using System;

namespace PersonManagementModule.ViewModels
{

  /// <summary>
  /// A PersonItem is a wrapper around a Person model class that makes it usable with a view. 
  /// In particular, it implements whatever is required for data bindings, error tracking, etc.
  /// </summary>
  public class PersonItem
  {
    /// <summary>Gets or sets the wrapped Person model.</summary>
    /// <value>The model.</value>
    public Person Model { get; set; }

    // TODO: do whatever is required for Property changes
    /// <summary>Gets or sets the person's first name.</summary>
    /// <value>The first name.</value>
    public string FirstName { get => Model.FirstName; set => Model.FirstName = value; }

    /// <summary>Gets or sets the person's last name.</summary>
    /// <value>The last name.</value>
    public string LastName { get => Model.LastName; set => Model.LastName = value; }

    /// <summary>Gets or sets the person's military title.</summary>
    /// <value>The title.</value>
    public string Title { get => Model.Title; set => Model.Title = value; }

    /// <summary>Initializes a new instance of the <see cref="PersonItem"/> class.
    /// The wrapped model must not be null.</summary>
    /// <param name="model">The model.</param>
    /// <exception cref="System.NullReferenceException"><paramref name="model" /> is null</exception>
    public PersonItem(Person model)
    {
      Model = model ?? throw new NullReferenceException("Wrapped person model cannot be null.");
    }
  }
}
