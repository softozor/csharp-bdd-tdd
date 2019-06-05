namespace DataAccess.Handlers
{
  public class FileHandlerFactory : IFileHandlerFactory
  {
    public IFileHandler Create(string filename, string fileType, bool append = false)
    {
      return new FileHandler(filename, fileType, append);
    }
  }
}
