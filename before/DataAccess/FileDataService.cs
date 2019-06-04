using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;

namespace DataAccess
{
  public class FileDataService : IDataService
  {
    public string Filename { get; private set; }
    public FileFormat FileType { get; private set; }

    public FileDataService()
    {
      var dbSettings = ConfigurationManager.GetSection("PersonMgmt/DatabaseSettings") as NameValueCollection;
      Filename = dbSettings["Filename"];
      FileType = (FileFormat)Enum.Parse(typeof(FileFormat), dbSettings["Type"]);
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public IEnumerable<Person> GetAllPersons()
    {
      throw new NotImplementedException();
    }

    /// <summary>Overwrites the currently persisted data with the provided <paramref name="persons" />.</summary>
    /// <param name="persons">List of the persons to be persisted. Must not be null.</param>
    /// <exception cref="System.NullReferenceException"><paramref name="persons" /> is null</exception>
    public void SavePersons(IEnumerable<Person> persons)
    {
      throw new NotImplementedException();
    }
  }
}
