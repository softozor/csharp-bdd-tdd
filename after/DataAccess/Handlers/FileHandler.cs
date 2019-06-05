using Models;
using System;
using System.Collections.Generic;

namespace DataAccess.Handlers
{
  public class FileHandler : IFileHandler
  {
    readonly bool _append;
    readonly string _filename;
    readonly FileFormat _fileType;

    public FileHandler(string filename, string fileType, bool append = false)
    {
      _append = append;
      _filename = filename;
      _fileType = (FileFormat)Enum.Parse(typeof(FileFormat), fileType);
    }

    public void WriteFile(IEnumerable<Person> persons)
    {
      switch (_fileType)
      {
        case FileFormat.JSON:
          JsonFileHandler.WritePersons(persons, _filename, _append);
          break;
        case FileFormat.XML:
        case FileFormat.CSV:
        default:
          throw new UnsupportedFileTypeException($"Unsupported file type <{_fileType.ToString()}>.");
      }
    }

    public IEnumerable<Person> ReadFile()
    {
      switch (_fileType)
      {
        case FileFormat.JSON:
          return JsonFileHandler.ReadPersons(_filename);
        case FileFormat.XML:
          break;
        case FileFormat.CSV:
          break;
        default:
          break;
      }
      throw new UnsupportedFileTypeException($"Unsupported file type <{_fileType.ToString()}>.");
    }
  }
}
