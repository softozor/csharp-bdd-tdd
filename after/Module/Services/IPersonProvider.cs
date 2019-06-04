using DataAccess;
using Models;
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
    IEnumerable<Person> GetPersons();

    /// <summary>Synchronizes the persisted data with the provided <paramref name="persons" />.</summary>
    /// <param name="persons">must not be null</param>
    /// <exception cref="System.NullReferenceException">
    ///   <paramref name="persons" /> is null</exception>
    void Save(IEnumerable<Person> persons);

    /// <summary>Imports data from the specified file into the database.
    /// Upon data importation, the database is augmented with the imported data. The data loaded from the file must be valid. The file must exist.
    /// The ids provided in the file are generated again to fit the database.</summary>
    /// <param name="filename">Full path to the file to be imported.
    /// The filename must not be null. The file pointed to by the filename must exist.</param>
    /// <param name="format">The file format <see cref="FileFormat"/></param>
    /// <exception cref="System.NullReferenceException">
    ///   <para><paramref name="filename"/> is null</para>
    /// </exception>
    /// <exception cref="System.IO.FileNotFoundException"><paramref name="filename"/> points to non-existing file</exception>
    /// <exception cref="InvalidDataFormatException">data stored in <paramref name="filename"/> are not consistent with the <paramref name="format"/></exception>
    /// <exception cref="InvalidDataStructureException">data stored in <paramref name="filename"/> are not compatible with the database (mismatching fields)</exception>
    void Import(string filename, FileFormat format);

    /// <summary>Exports the data stored in the database to the specified <paramref name="filename"/> in the specified <paramref name="format"/>. 
    /// If the file already exists, then it is overwritten.</summary>
    /// <param name="filename">Must not be empty.</param>
    /// <param name="format"><see cref="FileFormat"/></param>
    /// <exception cref="System.NotSupportedException">the filename points to an invalid location</exception>
    void Export(string filename, FileFormat format);
  }
}
