using BoDi;
using DataAccess.Handlers;
using DataAccess.Services;
using Microsoft.Practices.Unity;
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
      var bootstrapper = RunModule();
      SetupStepsDependencies(bootstrapper);
    }

    private void InitDatabase()
    {
      var overwriteIfExists = true;
      File.Copy(Path.GetFullPath(DB_FIXTURE), GetDatabaseFilename(), overwriteIfExists);
    }

    private Bootstrapper RunModule()
    {
      var bootstrapper = new Bootstrapper();
      bootstrapper.Run();
      return bootstrapper;
    }

    private void SetupStepsDependencies(Bootstrapper bootstrapper)
    {
      ExposeType<IDataService>(bootstrapper);
      ExposeType<IFileHandlerFactory>(bootstrapper);
      ExposeType<IPersonProvider>(bootstrapper);
    }

    private void ExposeType<T>(Bootstrapper bootstrapper) where T : class
    {
      var instance = bootstrapper.Container.Resolve<T>();
      _objectContainer.RegisterInstanceAs(instance);
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
