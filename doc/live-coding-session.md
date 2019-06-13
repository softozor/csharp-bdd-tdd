# Live coding session

The necessary steps towards the implementation of a feature are described in the following sections. The goal is to implement a [Prism](https://prismlibrary.github.io) module supporting the following feature.

## Goal

The goal of this presentation is to shortly introduce acceptance and unit-testing in C# / .NET / Prism. Things are kept easy: no view is implemented, focus is exclusively put on the module's logic. As a side-note, it would be possible to write acceptance tests for the view too, but that would be pretty time-consuming. There are interesting possibilities with the [White Testing Framework](https://teststackwhite.readthedocs.io/en/latest/) but that framework does not seem to be maintained any more and is fairly difficult to use when working with UI component bundles like [DevExpress](https://www.devexpress.com/) or [Telerik](https://www.telerik.com/). We recommend to rather implement such tests with [CodedUI](https://docs.microsoft.com/en-us/visualstudio/test/use-ui-automation-to-test-your-code?view=vs-2019) when necessary. However, would bugs be popping up in the view, tracking them with acceptance tests might be useful under certain circumstances. 

Therefore, let's focus on a Prism module's `ViewModel`'s implementation which needs to integrate in our case with a [DataAccess](../before/DataAccess) essentially. The [initial module's code](../before/Module) was already prepared. It is a Prism Module project generated from Visual Studio 2017's templates downgraded to support `Prism 6.3.0`.

## Feature to be implemented

```gherkin
Feature: Technical Officer manages persons
  As a Technical Officer or an Instructor
  I want to manage the list of the people using the simulator
  in order to produce access badges.

Background: The database is filled with persons
  
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

Scenario: The Technical Officer is browsing through the persons' list

  Then she has access to the persisted persons 

Scenario: The Technical Officer imports new persons

  Pure data importation has no effect on the underlying database's state.

  When the Technical Officer imports a list of persons 
  Then they are not persisted to the database
```

The next sections are more or less a transcript of the video that will be published by Softozor on the topic. In the description below, we use [SpecFlow](https://specflow.org) as it is very well integrated to Visual Studio. Any other framework would also perfectly fit to our presentation. You might e.g. be interested in double-checking the new [Gauge test automation tool](https://gauge.org), which happens to be free.

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

You will be provided with a feature file about a calculator adding two numbers. Just replace that specification with the above [feature](https://github.com/softozor/csharp-bdd-tdd/blob/master/doc/live-coding-session.md#feature-to-be-implemented). Copy the feature with the scenarios.

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

### Make the module available to the acceptance tests

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

### Setup and teardown the database

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

1. Add a new `Fixtures` folder to the `Spec` project where you put the above file. Make sure you copy that fixture file to the build folder by setting its properties accordingly:

![Copy fixture to build folder](/doc/img/CopyFixturesToBuildFolder.png)

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
Background: The database is filled with persons
  
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

In the above `GivenAListOfPersonsWasPersistedToTheDatabase` method, all the currently persisted persons are gathered and we check that the that persons' list isn't empty.

The problem in that method is that no `IDataService` instance has been provided yet. In fact, not exactly. The `PersonManagementModule.Module` has registered something:

```c#
// Module/Module.cs

public void Initialize()
{
  _container.RegisterType<IDataService, FileDataService>();
}
```

and the module is bootstrapped in the hooks. However, our acceptance tests won't know anything about that instance, because SpecFlow is using another [DI container](https://github.com/techtalk/SpecFlow/wiki/Context-Injection). Our current ansatz to provide our acceptance tests with the instances registered in our module is just to transfer them from one container to the other like this:

```c#
// Spec/Hooks.cs

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

Now, if any of the acceptance tests is run, the code of the background step is evaluated without errors and an exception is thrown because of a call to

```c#
_scenarioContext.Pending();
```

## Implementation of the first scenario: The Technical Officer manually persists a new person to the database

In order to implement the functionality in the BDD way, we will first write the acceptance test. Then we will write unit tests and production code in small iterations in the way depicted by the following picture:

![BDD -- TDD](/doc/img/bdd-tdd.png)

The success of one acceptance test will depend on the success of many unit tests supporting it.

### Our first acceptance test

Let's first focus on the `Given` step of the first scenario

```gherkin
Scenario: The Technical Officer manually persists a new person to the database

  Given the Technical Officer has added a new person
  When she saves
  Then the new person is persisted to the database 
```

Let's translate the `Given` statement to code:

```c#
// Spec/StepDefinitions/TechnicalOfficerManagesPersonsSteps.cs

[Given(@"the Technical Officer has added a new person")]
public void GivenTheTechnicalOfficerHasAddedANewPerson()
{
  var person = new Person
    {
      FirstName = "My FirstName",
      LastName = "My LastName",
      Title = "My Title"
    };
  var personItem = new PersonItem(person);
  _viewModel.Persons.Add(personItem);
  _scenarioContext.Set(person);
}
```

Basically, the idea is that the `PersonViewModel` wraps the POCO `Person` by mean of a `PersonItem` class. The purpose of that wrapper is to react to user input from a view, which will not be implemented here. The user somehow accesses a data grid view. Upon addition of an item to that grid view, an observable collection of persons, `PersonViewModel.Persons` is augmented with a new person. 

Additionally, because the scenario's `Then` step wants to assert something about that newly added person, it is kept in the scenario context so that it can be fetched later.

All that means the `PersonViewModel` needs to be initialized with the following code:

```c#
// Module/ViewModels/PersonViewModel.cs

using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PersonManagementModule.ViewModels
{
  public class PersonViewModel : BindableBase
  {
    public ObservableCollection<PersonItem> Persons { get; set; }

    public PersonViewModel()
    {
    }
  }
}
```

That needs the new `PersonItem` class:

```c#
// Module/ViewModels/PersonItem.cs

using Models;
using System;

namespace PersonManagementModule.ViewModels
{
  public class PersonItem
  {
    public Person Model { get; set; }

    public string FirstName { get => Model.FirstName; set => Model.FirstName = value; }

    public string LastName { get => Model.LastName; set => Model.LastName = value; }

    public string Title { get => Model.Title; set => Model.Title = value; }

    public PersonItem(Person model)
    {
      Model = model ?? throw new NullReferenceException("Wrapped person model cannot be null.");
    }
  }
}
```

Like that, the `PersonItem` is not ready to bind with a view. As already mentioned, that is not something we care about in this session. In a production settings, that `PersonItem` needs to notify about changes and keep track of data changes so that they are reflected in the model (see e.g. [WPF and MVVM: Advanced Model Treatment](https://app.pluralsight.com/library/courses/wpf-mvvm-advanced-model-treatment)). 

Now, let's translate the above `When` step into code. In order to save the persons to the database, a view will usually trigger a command. That is why the `When` step would naturally be translated into this:

```c#
// Spec/StepDefinitions/TechnicalOfficerManagesPersonsSteps.cs

[When(@"she saves")]
public void WhenSheSaves()
{
  _viewModel.SavePersonsCommand.Execute();
}
```

That needs addition of the `SavePersonsCommand` delegate command to the view model:

```c#
// Module/ViewModels/PersonViewModel.cs

using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;

namespace PersonManagementModule.ViewModels
{
  public class PersonViewModel : BindableBase
  {
    public ObservableCollection<PersonItem> Persons { get; set; }
    public DelegateCommand SavePersonsCommand { get; set; }

    public PersonViewModel()
    {
    }
  }
}
```

Last, we can translate the `Then` step to assertions. We want to check that the new person we added in the `Given` step actually is stored in the database: 

```c#
// Spec/StepDefinitions/TechnicalOfficerManagesPersonsSteps.cs

[Then(@"the new person is persisted to the database")]
public void ThenTheNewPersonIsPersistedToTheDatabase()
{
  var newPerson = _scenarioContext.Get<Person>();
  var persistedPersons = _dataService.GetAllPersons();
  var isNewPersonPersistedToDatabase = persistedPersons.Any(
    person => person.FirstName == newPerson.FirstName
      && person.LastName == newPerson.LastName
      && person.Title == newPerson.Title
  );
  Assert.IsTrue(isNewPersonPersistedToDatabase);
}
```

First the new added person is got from the scenario context. Then the persisted data are fetched, so that the presence of the new person can be verified there. 

Voilà. The first acceptance scenario has been implemented. Writing that first scenario clearly showed the whole point of BDD: we were developing in an outside-in fashion, asking ourselves what pieces of software would be required to achieve our goal. We came up with the idea of bringing an `ObservableCollection` of `PersonItem`s and a `DelegateCommand` into our view model. That settles the basis of our implementation.

### Acceptance test code improvements

We were able to write some acceptance test code. It is, however, somehow "polluted" with unnecessary details that might prevent another developer to rapidly understand what it is we want to do. For that reason, we would like to introduce a domain-specific test framework. In addition, we will encapsulate test data generation and use a tool for that.

#### Domain-specific test framework

Currently, it might not seem very uncomfortable to work with the code we came up with in our tests. However, with a growing suite of test scenarios, it is really necessary to introduce a domain-specific framework. Such a framework introduces helper classes / methods to improve the test code as well as its readability. In the case we are currently interested in, we think it is a good idea to introduce the following `PersonManager` that wraps the `PersonViewModel` under test:

```c#
// Spec/PersonManager.cs

using Models;
using PersonManagementModule.ViewModels;
using TestUtils;

namespace Spec
{
  public class PersonManager
  {
    readonly PersonViewModel _viewModel;

    public PersonManager(PersonViewModel viewModel)
    {
      _viewModel = viewModel;
    }

    public Person AddNewPerson()
    {
      var person = new Person
      {
        FirstName = "My FirstName",
        LastName = "My LastName",
        Title = "My Title"
      };
      var personItem = new PersonItem(person);
      _viewModel.Persons.Add(personItem);
      return person;
    }

    public void Save()
    {
      _viewModel.SavePersonsCommand.Execute();
    }
  }
}
```

That `PersonManager` class encapsulates away all the actions we want to perform in our tests. That triggers the following refactoring of the `TechnicalOfficerManagesPersonsSteps` class:

```c#
// Spec/StepDefinitions/TechnicalOfficerManagesPersonsSteps.cs

using DataAccess.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using System.Linq;
using TechTalk.SpecFlow;

namespace Spec.StepDefinitions
{
  [Binding]
  public class TechnicalOfficerManagesPersonsSteps
  {
    readonly ScenarioContext _scenarioContext;
    readonly IDataService _dataService;
    readonly PersonManager _personManager;

    public TechnicalOfficerManagesPersonsSteps(IDataService dataService, PersonManager personManager, ScenarioContext scenarioContext)
    {
      _personManager = personManager;
      _dataService = dataService;
      _scenarioContext = scenarioContext;
    }

    [Given(@"a list of persons was persisted to the database")]
    public void GivenAListOfPersonsWasPersistedToTheDatabase()
    {
      var persons = _dataService.GetAllPersons();
      Assert.IsTrue(persons.Count() > 0);
    }

    [Given(@"the Technical Officer has added a new person")]
    public void GivenTheTechnicalOfficerHasAddedANewPerson()
    {
      var person = _personManager.AddNewPerson();
      _scenarioContext.Set(person);
    }

    [When(@"she saves")]
    public void WhenSheSaves()
    {
      _personManager.Save();
    }

    [Then(@"the new person is persisted to the database")]
    public void ThenTheNewPersonIsPersistedToTheDatabase()
    {
      var newPerson = _scenarioContext.Get<Person>();
      var persistedPersons = _dataService.GetAllPersons();
      var isNewPersonPersistedToDatabase = persistedPersons.Any(
        person => person.FirstName == newPerson.FirstName
          && person.LastName == newPerson.LastName
          && person.Title == newPerson.Title
      );
      Assert.IsTrue(isNewPersonPersistedToDatabase);
    }
  }
}
```

Any new operation to be tested through the `PersonViewModel` will from now on go through the `PersonManager`. Note that the `PersonManager` is automatically added to SpecFlow's [BoDi container](https://github.com/gasparnagy/BoDi), hence nothing needs to be additionally configured in our `Hooks` class.

#### Test data generation with `NBuilder`

During our acceptance tests, it might be that we will need to generate test data. For example, we already needed to create a new person:

```c#
// Spec/PersonManager.cs

public Person AddNewPerson()
{
  var person = new Person
  {
    FirstName = "My FirstName",
    LastName = "My LastName",
    Title = "My Title"
  };
  var personItem = new PersonItem(person);
  _viewModel.Persons.Add(personItem);
  return person;
}
```

For some reasons, such code might be unpleasant. Indeed, we might want to add more than one person at some point and that would involve writing all the persons' properties by hand. Instead, what we suggest is to use the [NBuilder library](https://github.com/nbuilder/nbuilder) and to refactor the `PersonManager` like this:

```c#
// Spec/PersonManager.cs

using Models;
using PersonManagementModule.ViewModels;
using TestUtils;

namespace PersonManagementSpec
{
  public class PersonManager
  {
    readonly PersonViewModel _viewModel;
    readonly NewSinglePersonGenerator _personGenerator;

    public PersonManager(NewSinglePersonGenerator personGenerator, PersonViewModel viewModel)
    {
      _personGenerator = personGenerator;
      _viewModel = viewModel;
    }

    public Person AddNewPerson()
    {
      var person = _personGenerator.Generate();
      var personItem = new PersonItem(person);
      _viewModel.Persons.Add(personItem);
      return person;
    }

    [...]
  }
}
```

where the `NewSinglePersonGenerator` is part of the new `TestUtils` library which you need to add to the solution:

![Add TestUtils library](/doc/img/AddTestUtilsLibrary.png)

```c#
// TestUtils/NewSinglePersonGenerator.cs

using FizzWare.NBuilder;
using FizzWare.NBuilder.PropertyNaming;
using Models;

namespace TestUtils
{
  public class NewSinglePersonGenerator
  {
    public NewSinglePersonGenerator()
    {
      BuilderSetup.SetDefaultPropertyName(new RandomValuePropertyNamer(new BuilderSettings()));
    }

    public Person Generate()
    {
      return Builder<Person>.CreateNew().WithInvalidId().Build();
    }
  }
}
```

The `TestUtils` library needs a reference to the `NBuilder` NuGet package:

![NBulider NuGet package](/doc/img/NBuilderPackage.png)

In the above code snippet, we use `NBuilder` to generate a new random person with invalid id. The `WithInvalidId` method is an extension of us:

```c#
// TestUtils/TestDataBuilderExtensions.cs

using FizzWare.NBuilder;
using Models;

namespace TestUtils
{
  public static class TestDataBuilderExtensions
  {
    public static ISingleObjectBuilder<Person> WithInvalidId(this ISingleObjectBuilder<Person> builder)
    {
      return builder.With(person => person.Id = null);
    }
  }
}
```

The idea behind that extension method is to easily generate random persons with invalid ids. 

With all that in place, our code is much more readable and data generation is much easier.

### Production code implementation

The goal is now to let the previous acceptance scenario pass. Currently, it is not passing because most of the code is undefined. For example, the `PersonViewModel`'s `Persons` observable collection is `null` as well as its `SavePersonsCommand`.

In a first step, let's add a new unit test project to our solution:

![Add unit test project](/doc/img/AddUnitTestProject.png)

Make sure you have the following NuGet packages installed:

![Test project NuGet packages](/doc/img/TestProjectNuGetPackages.png)

The [Mock library](https://github.com/Moq/moq4/wiki/Quickstart) will prove to be very useful in the coming up unit tests.

Let's also rename `UnitTests1.cs` to `ViewModelTests.cs` and let's define our following test method:

```c#
// Tests/ViewModelTests.cs

/// <summary>
/// Upon saving of a new person, that new person needs to be persisted to the underlying database.
/// </summary>
[TestMethod]
public void ShouldPersistNewPerson()
{
  // Given -- Arrange
  var personProviderMock = new Mock<IPersonProvider>();
  var viewModel = new PersonViewModel(personProviderMock.Object);
  var personGenerator = new NewSinglePersonGenerator();
  var model = personGenerator.Generate();
  var personItem = new PersonItem(model);

  // When -- Act
  viewModel.Persons.Add(personItem);
  viewModel.SavePersonsCommand.Execute();

  // Then -- Assert
  var personsToBeSaved = from item in viewModel.Persons select item.Model;
  personProviderMock.Verify(provider => provider.Save(personsToBeSaved), Times.Once());
}
```

Here above, we first create whatever is necessary for our test: a `PersonViewModel` and a new `PersonItem` in essence. Then we act on the system under test: we verify that the data are persisted. Note that we use the `NewSinglePersonGenerator` we defined as we were writing the acceptance tests.

In the global code overview we came up with during the writing of the acceptance tests, we did not delve that much into the details of how our `PersonViewModel` would look like. The apparition of an `IPersonProvider` popped up upon writing that test. Indeed, the data need to come from somewhere, and that somewhere is precisely the `IPersonProvider`. The construction of the `PersonViewModel` needs to be adapted:

```c#
// Module/ViewModels/PersonViewModel.cs

public class PersonViewModel : BindableBase
{
  readonly IPersonProvider _personProvider;

  public ObservableCollection<PersonItem> Persons { get; set; }
  public DelegateCommand SavePersonsCommand { get; set; }

  public PersonViewModel(IPersonProvider personProvider)
  {
    _personProvider = personProvider;
  }
}
```

Additionally, the `IPersonProvider` needs to be defined:

```c#
// Module/Services/IPersonProvider.cs

using Models;
using System.Collections.Generic;

namespace PersonManagementModule.Services
{
  public interface IPersonProvider
  {
    IEnumerable<Person> GetPersons();

    void Save(IEnumerable<Person> persons);
  }
}
```

To let the `ViewModelTests.ShouldPersistNewPerson` test pass, we do not need to care about the `IPersonProvider` yet. Indeed, we mock it in the above test. Let's complete the `PersonViewModel` as follows:

```c#
// Module/ViewModels/PersonViewModel.cs

using PersonManagementModule.Services;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;

namespace PersonManagementModule.ViewModels
{
  public class PersonViewModel : BindableBase
  {
    readonly IPersonProvider _personProvider;

    public ObservableCollection<PersonItem> Persons { get; set; }

    public PersonViewModel(IPersonProvider personProvider)
    {
      _personProvider = personProvider;

      Persons = new ObservableCollection<PersonItem>();
      SavePersonsCommand = new DelegateCommand(SavePersons, CanSavePersons);
    }

    public DelegateCommand SavePersonsCommand { get; set; }

    private void SavePersons()
    {
      var personModels = from item in Persons select item.Model;
      _personProvider.Save(personModels);
    }

    private bool CanSavePersons()
    {
      return true;
    }
  }
}
```

Let's implement a `PersonProvider`. To do so, let's start by specifying its purpose with a few unit tests:

```c#
// Tests/PersonProviderTests.cs

using DataAccess.Services;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using System;

namespace Tests
{
  [TestClass]
  public class PersonProviderTests
  {
    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void SaveThrowsIfNullIsProvided()
    {
      // Given
      var dataServiceMock = new Mock<IDataService>();
      var provider = new PersonProvider(dataServiceMock.Object);

      // When
      provider.Save(null);

      // Then: the assertions are triggered by the ExpectedException attribute
    }

    [TestMethod]
    public void ShouldPersistPersonsUponSaving()
    {
      // Given
      var dataServiceMock = new Mock<IDataService>();
      var provider = new PersonProvider(dataServiceMock.Object);

      // When
      var persons = Builder<Person>.CreateListOfSize(10).Build();
      provider.Save(persons);

      // Then
      dataServiceMock.Verify(service => service.SavePersons(persons), Times.Once());
    }
  }
}
```

In essence, we do not want to accept to save a `null` list of `Persons` and we want to ensure that the relevant persistence API is called upon saving. The corresponding `PersonProvider` implementation goes as follows:

```c#
// Module/Services/PersonProvider.cs

using DataAccess.Services;
using Models;
using System;
using System.Collections.Generic;

namespace PersonManagementModule.Services
{
  public class PersonProvider : IPersonProvider
  {
    readonly IDataService _dataService;

    public PersonProvider(IDataService dataService)
    {
      _dataService = dataService;
    }

    public void Save(IEnumerable<Person> persons)
    {
      _dataService.SavePersons(persons ?? throw new NullReferenceException("Cannot save null list of persons"));
    }

    public IEnumerable<Person> GetPersons()
    {
      throw new NotImplementedException();
    }
  }
}
```

The acceptance tests need to be made aware of the `IPersonProvider`. Hence, our test hooks class get upgraded with

```c#
// Spec/Hooks.cs

private void SetupStepsDependencies(Bootstrapper bootstrapper)
{
  ExposeType<IDataService>(bootstrapper);
  ExposeType<IPersonProvider>(bootstrapper);
}
```

and our `Module` class registers our `PersonProvider` implementation:

```c#
// Module/Module.cs

public void Initialize()
{
  _container.RegisterType<IFileHandlerFactory, FileHandlerFactory>();
  _container.RegisterType<IDataService, FileDataService>();
  _container.RegisterType<IPersonProvider, PersonProvider>();
}
```

Upon implementation of two unit tests for the `PersonProvider` and one unit test for the `PersonViewModel`, we are now able to let the first acceptance test pass.

### Unit test code refactoring

Our `PersonProviderTests` exhibit some unpleasant code duplication:

```c#
// Tests/PersonProviderTests.cs

// Given
var dataServiceMock = new Mock<IDataService>();
var provider = new PersonProvider(dataServiceMock.Object);
```

That can be factored out in a test initialization method:

```c#
// Tests/PersonProviderTests.cs

[TestClass]
public class PersonProviderTests
{
  Mock<IDataService> _dataServiceMock;
  PersonProvider _provider;

  [TestInitialize]
  public void Initialize()
  {
    _dataServiceMock = new Mock<IDataService>();
    _provider = new PersonProvider(_dataServiceMock.Object);
  }

  [TestMethod]
  [ExpectedException(typeof(NullReferenceException))]
  public void SaveThrowsIfNullIsProvided()
  {
    // When
    _provider.Save(null);

    // Then: the assertions are triggered by the ExpectedException attribute
  }

  [TestMethod]
  public void ShouldPersistPersonsUponSaving()
  {
    // When
    var persons = Builder<Person>.CreateListOfSize(10).Build();
    _provider.Save(persons);

    // Then
    _dataServiceMock.Verify(service => service.SavePersons(persons), Times.Once());
  }
}
```

The same holds for the `ViewModelTests`, although that is not yet apparent. Let's refactor it to 

```c#
// Tests/ViewModelTests.cs

[TestClass]
public class ViewModelTests
{
  Mock<IPersonProvider> _personProviderMock;
  PersonViewModel _viewModel;

  [TestInitialize]
  public void Init()
  {
    InitializeViewModel();
  }

  private void InitializeViewModel()
  {
    _personProviderMock = new Mock<IPersonProvider>();
    _viewModel = new PersonViewModel(_personProviderMock.Object);
  }

  [TestMethod]
  public void ShouldPersistNewPerson()
  {
    // Given
    var personGenerator = new NewSinglePersonGenerator();
    var model = personGenerator.Generate();
    var personItem = new PersonItem(model);

    // When
    _viewModel.Persons.Add(personItem);
    _viewModel.SavePersonsCommand.Execute();

    // Then
    var personsToBeSaved = from item in _viewModel.Persons select item.Model;
    _personProviderMock.Verify(provider => provider.Save(personsToBeSaved), Times.Once());
  }
}
```

## Implementation of the second scenario: The Technical Officer is browsing through the persons' list

### The acceptance test

That one is an easy acceptance test, because it consists of only an assertion step:

```gherkin
Scenario: The Technical Officer is browsing through the persons' list

  Then she has access to the persisted persons
```

First, let's introduce a new member variable `_viewModel` into the `TechnicalOfficerManagesPersonsSteps` class, as the above `Then` step will make use of it:

```c#
// Spec/StepDefinitions/TechnicalOfficerManagesPersonsSteps.cs

namespace Spec.StepDefinitions
{
  [Binding]
  public class TechnicalOfficerManagesPersonsSteps
  {
    readonly ScenarioContext _scenarioContext;
    readonly IDataService _dataService;
    readonly PersonViewModel _viewModel;

    public TechnicalOfficerManagesPersonsSteps(IDataService dataService, PersonViewModel viewModel, ScenarioContext scenarioContext)
    {
      _viewModel = viewModel;
      _dataService = dataService;
      _scenarioContext = scenarioContext;
    }

    [...]
  }
}
```

The above `Then` step can be translated to code in the following way:

```c#
// Spec/StepDefinitions/TechnicalOfficerManagesPersonsSteps.cs

[Then(@"she has access to the persisted persons")]
public void ThenSheHasAccessToThePersistedPersons()
{
  var persistedPersons = _dataService.GetAllPersons().OrderBy(person => person.Id);
  var accessiblePersons = _personManager.GetAccessiblePersons().OrderBy(person => person.Id);
  CollectionAssert.AreEqual(persistedPersons.ToList(), accessiblePersons.ToList());
}
```

where the `PersonManager` was upgraded with 

```c#
// Spec/PersonManager.cs

public IEnumerable<Person> GetAccessiblePersons()
{
  return from personItem in _viewModel.Persons select personItem.Model;
}
```

### Production code implementation

We need to ensure that the persons' data are loaded upon construction of the `PersonViewModel`. Let's first modify our `ViewModelTests` initialization a bit:

```c#
// Tests/ViewModelTests.cs

private void InitializeViewModel()
{
  _personProviderMock = new Mock<IPersonProvider>();
  _personProviderMock.Setup(provider => provider.GetPersons())
    .Returns(Builder<Person>.CreateListOfSize(10).Build());
  _viewModel = new PersonViewModel(_personProviderMock.Object);
}
```

That assumes there are data persisted to the database. Then, write a unit test checking that the persisted data are accessible from the `PersonViewModel`:

```c#
// Tests/ViewModelTests.cs

[TestMethod]
public void ShouldDisplayPersistedPersons()
{
  // Given
  var persistedPersons = _personProviderMock.Object.GetPersons();

  // When
  var accessiblePersons = from personItem in _viewModel.Persons select personItem.Model;

  // Then
  CollectionAssert.AreEqual(persistedPersons.ToList(), accessiblePersons.ToList());
}
```

Of course, that test does not pass because no data is loaded upon `PersonViewModel` construction. Let's fix that:

```c#
// Module/ViewModels/PersonViewModel.cs

public class PersonViewModel : BindableBase
{
  [...]

  public PersonViewModel(IPersonProvider personProvider)
  {
    _personProvider = personProvider;

    LoadPersons();

    SavePersonsCommand = new DelegateCommand(SavePersons, CanSavePersons);
  }

  private void LoadPersons()
  {
    var personItemList = from model in _personProvider.GetPersons() select new PersonItem(model);
    Persons = new ObservableCollection<PersonItem>(personItemList);
  }

  [...]
}
```

## Implementation of the last scenario: The Technical Officer imports new persons

### The acceptance test

Let's tackle the last scenario now:

```gherkin
Scenario: The Technical Officer imports new persons

  When the Technical Officer imports a list of persons 
  Then they are not persisted to the database
```

Let's put ourselves in the situation of the Technical Officer. She has a file that she wants to import. Let's first add that file to our `Fixtures` folder:

```json
{
  "DataItems": [
    {
      "Id": 11,
      "FirstName": "Laurent",
      "LastName": "Michel",
      "Title": "SW Architect"
    },
    {
      "Id": 22,
      "FirstName": "Thomas",
      "LastName": "Michel",
      "Title": "Mr Security"
    },
    {
      "Id": 33,
      "FirstName": "Florian",
      "LastName": "Pittet",
      "Title": "Artist"
    },
    {
      "Id": 44,
      "FirstName": "Cédric",
      "LastName": "Donner",
      "Title": "Web Technologies Expert"
    }
  ]
}
```

which you make sure to copy upon build, as the other fixture file `TestPersonsDatabase.json`. Then you reference it in the `TechnicalOfficerManagesPersonsSteps` class:

```c#
// Spec/TechnicalOfficerManagesPersonsSteps.cs

namespace Spec.StepDefinitions
{
  [Binding]
  public class TechnicalOfficerManagesPersonsSteps
  {
    const string FILE_TO_IMPORT = "Fixtures/PersonsToImport.json";

    [...]
  }
}
```

Now, the above steps can be translated to code as follows:

```c#
// Spec/TechnicalOfficerManagesPersonsSteps.cs

 [When(@"the Technical Officer imports a list of persons")]
public void WhenTheTechnicalOfficerImportsAListOfPersons()
{
  _scenarioContext.Set(_dataService.GetAllPersons().Count(), "initialPersonsCount");
  _personManager.Import(FILE_TO_IMPORT);
  var personsToImport = JsonFileHandler.ReadPersons(FILE_TO_IMPORT);
  Assert.IsTrue(personsToImport.Count() > 0);
}

[Then(@"they are not persisted to the database")]
public void ThenTheyAreNotPersistedToTheDatabase()
{
  var expectedPersonsCount = _scenarioContext.Get<int>("initialPersonsCount");
  var actualPersonsCount = _dataService.GetAllPersons().Count();
  Assert.AreEqual(expectedPersonsCount, actualPersonsCount);
}
```

where the `PersonManager` is augmented with

```c#
// Spec/PersonManager.cs

public void Import(string filename)
{
  var payload = new ImportPayload()
  {
    Filename = filename,
    FileType = "JSON"
  };
  _viewModel.ImportPersonsCommand.Execute(payload);
}
```

The view that will bind to our model will need to specify a filename and a file type. Then, invoking the `ImportPersonsCommand` would trigger whatever is necessary. Let's already define that command in the `PersonViewModel`:

```c#
// Module/ViewModels/PersonViewModel.cs

namespace PersonManagementModule.ViewModels
{
  public class PersonViewModel : BindableBase
  {
    [...]
    public DelegateCommand<ImportPayload> ImportPersonsCommand { get; set; }
    [...]
  }
}
```

as well as the `ImportPayload`

```c#
// Module/ViewModels/ImportPayload.cs

namespace PersonManagementModule.ViewModels
{
  public class ImportPayload
  {
    public string Filename { get; set; }

    public string FileType { get; set; }
  }
}
```

### Production code implementation

As usual, let's support the above acceptance test with a unit test:

```c#
[TestMethod]
public void ShouldAddImportedPersonsToObservableCollection()
{
  // Given
  var fileHandlerMock = new Mock<IFileHandler>();
  fileHandlerMock.Setup(handler => handler.ReadFile())
    .Returns(Builder<Person>.CreateListOfSize(10, new RandomValuePropertyNamer(new BuilderSettings())).Build());
  _fileHandlerFactoryMock.Setup(factory => factory.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
    .Returns(fileHandlerMock.Object);

  var listOfNewPersons = Enumerable.Empty<Person>();
  _viewModel.Persons.CollectionChanged += (s, e) =>
  {
    var newPersonItem = e.NewItems[0] as PersonItem;
    listOfNewPersons = listOfNewPersons.Append(newPersonItem.Model);
  };
  var persons = fileHandlerMock.Object.ReadFile();

  // When
  ImportPersonsFromFile();

  // Then
  CollectionAssert.AreEqual(persons.ToList(), listOfNewPersons.ToList());
}

private void ImportPersonsFromFile()
{
  var payload = new ImportPayload
  {
    Filename = "MyRandomFilename.json",
    FileType = "Json"
  };
  _viewModel.ImportPersonsCommand.Execute(payload);
}
```

First, we figured we'd need some kind of file handler that would read the file to be imported and interpret it as specified by its type. That file handler is already available in our `DataAccess` library. Setting up that handler is a bit trickier than anything we've done in the previous scenarios, which is the reason why we included that scenario to the session. Then, upon items addition to the `Persons` collection, we just add the new items to a list that we will use later in the assertions' section. After that, we import our data. The actual filename is meaningless, as we mocked the `FileHandlerFactory`: whatever its filename argument, the `fileHandlerMock.Object` is returned which will return the built list of 10 `Person`s with randomly filled properties. Finally, the difference between the list of added `Person`s to the `PersonViewModel` and that of `Person`s to be imported is asserted to be empty.

In order to make that unit test pass, we updated the `PersonViewModel` like this:

```c#
// Module/ViewModels/PersonViewModel.cs

public class PersonViewModel : BindableBase
{
  readonly IFileHandlerFactory _fileHandlerFactory;

  [...]

  public PersonViewModel(IFileHandlerFactory fileHandlerFactory, IPersonProvider personProvider)
  {
    _fileHandlerFactory = fileHandlerFactory;
    _personProvider = personProvider;

    LoadPersons();

    SavePersonsCommand = new DelegateCommand(SavePersons, CanSavePersons);
    ImportPersonsCommand = new DelegateCommand<ImportPayload>(ImportPersons, CanImportPersons);
  }

  [...]

  public DelegateCommand<ImportPayload> ImportPersonsCommand { get; set; }

  private void ImportPersons(ImportPayload payload)
  {
    var fileHandler = _fileHandlerFactory.Create(payload.Filename, payload.FileType);
    var persons = fileHandler.ReadFile();
    var newPersonItems = from person in persons select new PersonItem(person);
    Persons.AddRange(newPersonItems);
  }

  private bool CanImportPersons(ImportPayload payload)
  {
    return true;
  }
}
```

And with that, all the acceptance and unit tests are passing.