using PersonManagementModule.ViewModels;
using System.Collections.Generic;

namespace PersonManagementModule.Services
{

  /// <summary>
  ///   <para>An IPersonProvider is a service providing Persons. It acts as a proxy to the underlying Persons' database. </para>
  /// </summary>
  public interface IPersonProvider
  {
    /// <summary>Gets the persons currently stored in the underlying database.</summary>
    /// <returns>Enumerable.Empty&lt;Person&gt;() if the database is empty.</returns>
    IEnumerable<PersonItem> GetPersons();

    /// <summary>Synchronizes the persisted data with the provided <paramref name="persons" />.</summary>
    /// <param name="persons">must not be null</param>
    /// <exception cref="System.NullReferenceException">
    ///   <paramref name="persons" /> is null</exception>
    void Save(IEnumerable<PersonItem> persons);
  }
}
