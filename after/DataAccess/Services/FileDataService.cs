using DataAccess.Handlers;
using Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;

namespace DataAccess.Services
{
  public class FileDataService : IDataService
  {
    readonly IFileHandler _fileHandler;

    public FileDataService(IFileHandlerFactory factory)
    {
      var dbSettings = ConfigurationManager.GetSection("PersonMgmt/DatabaseSettings") as NameValueCollection;
      var filename = dbSettings["Filename"];
      var fileType = dbSettings["Type"];
      _fileHandler = factory.Create(filename, fileType);
    }

    public void Dispose()
    {
    }

    public IEnumerable<Person> GetAllPersons()
    {
      return _fileHandler.ReadFile();
    }

    /// <summary>Overwrites the currently persisted data with the provided <paramref name="persons" />.</summary>
    /// <param name="persons">List of the persons to be persisted. Must not be null.</param>
    /// <exception cref="System.NullReferenceException"><paramref name="persons" /> is null</exception>
    public void SavePersons(IEnumerable<Person> persons)
    {
      _fileHandler.WriteFile(persons ?? throw new NullReferenceException());
    }
  }
}
