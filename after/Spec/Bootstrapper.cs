using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;

namespace PersonManagementSpec
{
  sealed class Bootstrapper : UnityBootstrapper
  {
    protected override IModuleCatalog CreateModuleCatalog()
    {
      return new ConfigurationModuleCatalog();
    }
    protected override void ConfigureModuleCatalog()
    {
      var moduleCatalog = ModuleCatalog as ModuleCatalog;
      try
      {
        moduleCatalog.AddModule(typeof(PersonManagementModule.Module));
      }
      catch (Exception ex)
      {
        LogFatalException(ex);
      }
    }

    private void LogFatalException(Exception ex)
    {
      Logger.Log($"Fatal error has occurred {ex.ToString()}", Prism.Logging.Category.Exception, Prism.Logging.Priority.High);
    }

    protected override void ConfigureContainer()
    {
      try
      {
        base.ConfigureContainer();
      }
      catch (Exception ex)
      {
        LogFatalException(ex);
      }
    }
    protected override DependencyObject CreateShell()
    {
      // In our integration tests, we don't want to interact with the module's UI, 
      // therefore we don't initialize it
      return null;
    }
  }
}
