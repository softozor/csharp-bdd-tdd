using BoDi;
using DataAccess.Handlers;
using DataAccess.Services;
using PersonManagementModule.Services;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using TechTalk.SpecFlow;

namespace Spec
{
  [Binding]
  public sealed class Hooks
  {
    const string DB_FIXTURE = "Fixtures/TestPersonsDatabase.json";

    readonly IObjectContainer _objectContainer;

    public Hooks(IObjectContainer objectContainer)
    {
      _objectContainer = objectContainer;
    }

    [BeforeScenario]
    public void BeforeScenario()
    {
      InitDatabase();
      SetupStepsDependencies();
    }

    private void InitDatabase()
    {
      var overwriteIfExists = true;
      File.Copy(Path.GetFullPath(DB_FIXTURE), GetDatabaseFilename(), overwriteIfExists);
    }

    private void SetupStepsDependencies()
    {
      _objectContainer.RegisterTypeAs<FileDataService, IDataService>();
      _objectContainer.RegisterTypeAs<FileHandlerFactory, IFileHandlerFactory>();
      _objectContainer.RegisterTypeAs<PersonProvider, IPersonProvider>();
    }

    [AfterScenario]
    public void AfterScenario()
    {
      CleanupDatabase();
    }

    private void CleanupDatabase()
    {
      File.Delete(GetDatabaseFilename());
    }

    private static string GetDatabaseFilename()
    {
      var dbSettings = ConfigurationManager.GetSection("PersonMgmt/DatabaseSettings") as NameValueCollection;
      var dbFilename = dbSettings["Filename"];
      return dbFilename;
    }
  }
}
