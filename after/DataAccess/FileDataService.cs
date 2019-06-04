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
      return ReadFile();
    }

    /// <summary>Overwrites the currently persisted data with the provided <paramref name="persons" />.</summary>
    /// <param name="persons">List of the persons to be persisted. Must not be null.</param>
    /// <exception cref="System.NullReferenceException"><paramref name="persons" /> is null</exception>
    public void SavePersons(IEnumerable<Person> persons)
    {
      if(persons == null)
        throw new NullReferenceException();
      persons = IndexPersons(persons);
      WriteFile(persons);
    }

    IEnumerable<Person> IndexPersons(IEnumerable<Person> persons)
    {
      var startIdx = 0;
      return persons.Select(person => 
      {
        person.Id = startIdx++;
        return person;
      });
    }

    private void WriteFile(IEnumerable<Person> persons)
    {
      switch (FileType)
      {
        case FileFormat.JSON:
          WritePersonsToJson(Filename, persons);
          break;
        case FileFormat.XML:
        case FileFormat.CSV:
        default:
          throw new UnsupportedFileTypeException($"Unsupported file type <{FileType.ToString()}>.");
      }
    }

    private void WritePersonsToJson(string filename, IEnumerable<Person> persons)
    {
      var append = false;
      using (var w = new StreamWriter(filename, append))
      {
        var personsList = new PersonsList()
        {
          DataItems = persons
        };
        var serializedList = JsonConvert.SerializeObject(personsList);
        w.Write(serializedList);
      }
    }

    private IEnumerable<Person> ReadFile()
    {
      switch (FileType)
      {
        case FileFormat.JSON:
          return ExtractPersonsFromJson(Filename);
        case FileFormat.XML:
          break;
        case FileFormat.CSV:
          break;
        default:
          break;
      }
      throw new UnsupportedFileTypeException($"Unsupported file type <{FileType.ToString()}>.");
    }

    private static IEnumerable<Person> ExtractPersonsFromJson(string filename)
    {
      using (var r = new StreamReader(filename))
      {
        var json = r.ReadToEnd();
        var persons = JsonConvert.DeserializeObject<PersonsList>(json);
        return persons.DataItems;
      }
    }
  }
}
