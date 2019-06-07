# Live coding session plan

The necessary steps towards the implementation of a feature are described in the following sections. The goal is to implement a [Prism](https://prismlibrary.github.io) module supporting the following feature:

```gherkin
Feature: Technical Officer manages persons
  As a Technical Officer or an Instructor
  I want to manage the list of the people using the simulator
  in order to produce access badges.

Background: The Technical Officer is browsing through the persons' list
  
  Given a list of persons was persisted to the database 

Scenario: The Technical Officer manually persists a new person to the database

  In addition to importing person data from a file, the Technical Officer can 
  add persons to the system manually. First, the Technical Officer needs to 
  add a new person. That person is then stored in the application. The Technical 
  Officer finally needs to save in order to definitely persist the person to 
  the database. 

  Given the Technical Officer has added a new person
  When she saves
  Then the new person is persisted to the database 

Scenario: The Technical Officer imports new persons

  Pure data importation has no effect on the underlying database's state.

  When the Technical Officer imports a list of persons 
  Then they are not persisted to the database
```

The next sections are more or less a transcript of the video that will be published by Softozor on the topic. In the description below, we use [SpecFlow](https://specflow.org) as it is very well integrated to Visual Studio. Any other framework would also perfectly fit to our presentation. You might e.g. be interested in double-checking the new [Gauge test automation tool](https://gauge.org), which happens to be free.

## Goal

The goal of this presentation is to shortly introduce acceptance and unit-testing in C# / .NET / Prism. Things are kept easy: no view is implemented, focus is exclusively put on the module's logic. As a side-note, it would be possible to write acceptance tests for the view too, but that would be pretty time-consuming. There are interesting possibilities with the [White Testing Framework](https://teststackwhite.readthedocs.io/en/latest/) but that framework does not seem to be maintained any more and is fairly difficult to use when working with UI component bundles like [DevExpress](https://www.devexpress.com/) or [Telerik](https://www.telerik.com/). We recommend to rather implement such tests with [CodedUI](https://docs.microsoft.com/en-us/visualstudio/test/use-ui-automation-to-test-your-code?view=vs-2019) when necessary. However, would bugs be popping up in the view, tracking them with acceptance tests might be useful under certain circumstances. 

Therefore, let's focus on a Prism module's `ViewModel`'s implementation which needs to integrate in our case with a [DataAccess](../before/DataAccess) essentially. The [initial module's code](../before/Module) was already prepared. It is a Prism Module project generated from Visual Studio 2017's templates downgraded to support `Prism 6.3.0`.

## Acceptance tests preparation

### Visual Studio project configuration

1. Open the pre-defined [solution](../before/PersonManagementApp.sln) with Visual Studio 2017.

2. Create a new "Unit Test Project (.NET Framework)" with name `Spec`:

![New SpecFlow project](/doc/img/NewSpecFlowProject.png)

By default, the project uses MSTest. 

3. Get rid of the `UnitTest1.cs` that is automatically added to the project.

4. Install SpecFlow in the `Spec` project:

![Install SpecFlow](/doc/img/InstallSpecFlow.png)

5. Add a reference to the TechTalk.SpecFlow dll **[IS THAT REALLY NECESSARY?]** :

![Add TechTalk SpecFlow reference](/doc/img/AddTechTalkSpecFlowReference.png)

6. Install `Prism.Unity` (6.3.0) NuGet package in the `Spec` project.

7. Provide the `Spec` project with a reference to the `Module`, `DataAccess`, and `Models` projects.

8. Add a new folder `Features` to the `Spec` project.

9. Create a folder `StepDefinitions` in the `Spec` project.

### Feature initialization

1. Add a new "SpecFlow Feature File" to the `Features` folder of the `Spec` project: 

![Add new SpecFlow Feature File](/doc/img/AddNewSpecFlowFeatureFile.png)

You will be provided with a feature file about a calculator adding two numbers. Just replace that specification with the above `Feature: Technical Officer manages persons`. Copy the feature with the scenarios.

Also, make sure to disable SpecFlow's single file generation:

![Disable SpecFlow Single File Generator](/doc/img/EnableSpecFlowSingleFileGenerator.png)

And verify that the `PersonManagement.feature` file's properties look like this:

![Get rid of SpecFlow custom tool](/doc/img/GetRidOfCustomTool.png)

2. Initially, the feature file looks like this:

![Initial Feature File](/doc/img/InitialPersonManagementFeatureFile.png)

You surely noticed that all the `Given`, `When`, `Then` steps are highlighted in purple. This means that the underlying step definition code does not exist yet. 

3. Generate the initial step definitions with a right-click on the feature file, then "Generate Step Definitions":

![Generate initial step definitions](/doc/img/GenerateInitialStepDefinitions.png)

![Step definition dialog](/doc/img/StepDefinitionGenerationDialog.png)

Store the step definition code file in the `StepDefinitions` folder of the `Spec` project. Upon step definitions generation, the step definitions are not purple any more in the feature file.

A typical generated step definition looks like this (depending on your SpecFlow version):

```c#
[Given(@"a list of persons was persisted to the database")]
public void GivenAListOfPersonsWasPersistedToTheDatabase()
{
  ScenarioContext.Current.Pending();
}
```

In that case, you might see the warning

![Obsolete ScenarioContext.Current](/doc/img/ObsoleteScenarioContextCurrent.png)

in which case you would just inject the `ScenarioContext` into the `TechnicalOfficerManagesPersonSteps` constructor:

```c#
readonly ScenarioContext _scenarioContext;

public TechnicalOfficerManagesPersonsSteps(ScenarioContext scenarioContext)
{
  _scenarioContext = scenarioContext;
}
```

and replace 

```c#
ScenarioContext.Current.Pending();
```

with

```c#
_scenarioContext.Pending();
```

## Make the module available to the acceptance tests

The `Spec` project builds an application that will glue together all the components necessary for our module to run. In particular, it needs to bootstrap our module. Because all the acceptance tests should be independent of each others, that means the module needs to be bootstrapped before each SpecFlow scenario. This is done by defining so-called [hooks](https://specflow.org/documentation/Hooks/). To boostrap our module in our acceptance tests, perform the following steps:

1. Add a SpecFlow hook to the `Spec` project:

![Add SpecFlow Hook](/doc/img/AddSpecFlowHook.png)

2. Run the module in the `BeforeScenario` hook:

```c#
// Spec/Hooks.cs

using TechTalk.SpecFlow;

namespace Spec
{
  [Binding]
  public sealed class Hooks
  {
    [BeforeScenario]
    public void BeforeScenario()
    {
      RunModule();
    }

    private void RunModule()
    {
      var bootstrapper = new Bootstrapper();
      bootstrapper.Run();
    }
  }
}

```

3. Create the `Bootstrapper` class in the `Spec` project:

```c#
// Spec/Bootstrapper.cs

using Prism.Modularity;
using Prism.Unity;
using System;
using System.Windows;

namespace Spec
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
```

## Setup and teardown the database

As the module, the test database needs to be initialized and teared down during every acceptance scenario. The database chosen for this example is just a simple text file. The whole database to be used in this example is this:

```json
// Spec/Fixtures/TestPersonsDatabase.json

{
  "DataItems": [
    {
      "Id": 1,
      "FirstName": "Lorenzo",
      "LastName": "Miguel",
      "Title": "Dr"
    },
    {
      "Id": 2,
      "FirstName": "Salvatore",
      "LastName": "Adamo",
      "Title": "Singer"
    }
  ]
}
```

In order to setup and tear-down the database for each scenario, we proceed this way:

1. Add a new `Fixtures` folder to the `Spec` project where you put the above file.

2. The database is interfaced by the [IDataService](../before/DataAccess/Services/IDataService.cs). Any parameter related to the database (e.g. the database's url) is provided in the application that will use the module. The used implementation of that interface, the [FileDataService](../before/DataAccess/Services/FileDataService.cs), is constructed in this way:

```c#
public FileDataService(IFileHandlerFactory factory)
{
  var dbSettings = ConfigurationManager.GetSection("PersonMgmt/DatabaseSettings") as NameValueCollection;
  var filename = dbSettings["Filename"];
  var fileType = dbSettings["Type"];
  _fileHandler = factory.Create(filename, fileType);
}
```

That means that our `Spec` project needs to provide that piece of information:

```xml
<!-- Spec/App.config -->
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
  <configSections>
    <sectionGroup name="PersonMgmt">
      <section name="DatabaseSettings" type="System.Configuration.NameValueSectionHandler"/>
    </sectionGroup>
  </configSections>
  <PersonMgmt>
    <DatabaseSettings>
      <add key="Filename" value="TestPersonsDatabase.json"/>
      <add key="Type" value="JSON"/>
    </DatabaseSettings>
  </PersonMgmt>
</configuration>
```

3. To avoid side-effects on our database, we're better off copying our `TestPersonsDatabase.json` during our tests and deleting the `TestPersonsDatabase.json` upon scenario completion. That is exactly what the following `BeforeScenario` and `AfterScenario` hooks are all about:

```c#
// Spec/Hooks.cs

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

    [BeforeScenario]
    public void BeforeScenario()
    {
      InitDatabase();
      RunModule();
    }

    private void InitDatabase()
    {
      var overwriteIfExists = true;
      File.Copy(Path.GetFullPath(DB_FIXTURE), GetDatabaseFilename(), overwriteIfExists);
    }

    private void RunModule()
    {
      var bootstrapper = new Bootstrapper();
      bootstrapper.Run();
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
```

## Background step

First of all, note that a so-called background step is evaluated before each scenario of the current feature. In our example, this is: 

```gherkin
Background: The Technical Officer is browsing through the persons' list
  
  Given a list of persons was persisted to the database
```

Its implementation requires the `IDataService`. Hence the `TechnicalOfficerManagesPersonSteps` needs to be updated to

```c#
// Spec/StepDefinitions/TechnicalOfficerManagesPersonSteps.cs

using DataAccess.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TechTalk.SpecFlow;

namespace Spec.StepDefinitions
{
  [Binding]
  public class TechnicalOfficerManagesPersonSteps
  {
    readonly ScenarioContext _scenarioContext;
    readonly IDataService _dataService;

    public TechnicalOfficerManagesPersonSteps(IDataService dataService, ScenarioContext scenarioContext)
    {
      _dataService = dataService;
      _scenarioContext = scenarioContext;
    }

    [Given(@"a list of persons was persisted to the database")]
    public void GivenAListOfPersonsWasPersistedToTheDatabase()
    {
      var persons = _dataService.GetAllPersons();
      Assert.IsTrue(persons.Count() > 0);
    }

    [...]
  }
}
```

The problem here is that no `IDataService` instance has been provided yet. In fact, not exactly. The `PersonManagementModule.Module` has registered something:

```c#
public void Initialize()
{
  _container.RegisterType<IDataService, FileDataService>();
}
```

and the module is bootstrapped in the hooks. However, our acceptance tests won't know anything about that instance, because SpecFlow is using another [DI container](https://github.com/techtalk/SpecFlow/wiki/Context-Injection). Our current ansatz to provide our acceptance tests with the instances registered in our module is just to transfer them from one container to the other like this:

```c#
private void ExposeType<T>(Bootstrapper bootstrapper) where T : class
{
  var instance = bootstrapper.Container.Resolve<T>();
  _objectContainer.RegisterInstanceAs(instance);
}
```

First, we resolve the interface's instance. Then, we put it into our acceptance tests' object container. There might be a more convenient way of dealing with that by means of the [SpecFlow plugins](https://github.com/techtalk/SpecFlow/wiki/Context-Injection#custom-dependency-injection-frameworks).

Following our ansatz, our hooks class becomes:

```c#
// Spec/Hooks.cs

using BoDi;
using DataAccess.Services;
using Microsoft.Practices.Unity;
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
```
