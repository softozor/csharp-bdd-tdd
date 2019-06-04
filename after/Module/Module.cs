using DataAccess;
using Microsoft.Practices.Unity;
using PersonManagementModule.Services;
using Prism.Modularity;

namespace PersonManagementModule
{
  public class Module : IModule
  {
    readonly IUnityContainer _container;

    public Module(IUnityContainer container)
    {
      _container = container;
    }

    public void Initialize()
    {
      _container.RegisterType<IPersonProvider, PersonProvider>();
      _container.RegisterType<IDataService, FileDataService>();
    }
  }
}