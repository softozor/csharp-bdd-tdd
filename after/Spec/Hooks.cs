using BoDi;
using DataAccess.Handlers;
using Microsoft.Practices.Unity;
using PersonManagementModule.Services;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using TechTalk.SpecFlow;

namespace PersonManagementSpec
{
  /// <summary>
  /// The Hooks class configures the necessary scenarios' setup and tear-down. In particular, 
  /// it is responsible for running the module by means of an integration bootstrapper.
  /// </summary>
  [Binding]
  sealed class Hooks
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
      ExposeType<IPersonProvider>(bootstrapper);
      ExposeType<IFileHandlerFactory>(bootstrapper);
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
