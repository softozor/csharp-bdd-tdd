using Microsoft.Practices.Unity;
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
    }
  }
}