using Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace DataAccess.Handlers
{
  static public class JsonFileHandler
  {
    static public void WritePersons(IEnumerable<Person> persons, string filename, bool append = false)
    {
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

    static public IEnumerable<Person> ReadPersons(string filename)
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
