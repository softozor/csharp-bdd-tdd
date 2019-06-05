using DataAccess.Handlers;

namespace DataAccess.Handlers
{
  public interface IFileHandlerFactory
  {
    IFileHandler Create(string filename, string fileType, bool append = false);
  }
}
