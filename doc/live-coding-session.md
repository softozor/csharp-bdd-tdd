# Live coding session plan

The necessary steps towards the implementation of a feature are described in the following sections. The goal is to implement a [Prism](https://prismlibrary.github.io) module supporting the following feature:

```
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

The goal of this presentation is to shortly introduce acceptance and unit-testing in C# / .NET / Prism. We keep things easy: no view is implemented, we exclusively focus on the module's logic. As a side-note, it would be possible to write acceptance tests for the view too, but that would be pretty time-consuming. There are interesting possibilities with the [White Testing Framework](https://teststackwhite.readthedocs.io/en/latest/) but that framework does not seem to be maintained any more and is fairly difficult to use when working with UI component bundles like [DevExpress](https://www.devexpress.com/) or [Telerik](https://www.telerik.com/). We recommend to rather implement such tests with [CodedUI](https://docs.microsoft.com/en-us/visualstudio/test/use-ui-automation-to-test-your-code?view=vs-2019) when necessary. Also, would bugs be popping up in the view, tracking them with acceptance tests might be useful under certain circumstances. 

Therefore, let's focus on a Prism module's `ViewModel`'s implementation which needs to integrate in our case with a [DataAccess](https://github.com/softozor/csharp-bdd-tdd/before/DataAccess) essentially. The [initial module's code](https://github.com/softozor/csharp-bdd-tdd/before/Module) was already prepared. It is a Prism Module project generated from Visual Studio 2017's templates downgraded to support `Prism 6.3.0`.

## Acceptance tests preparation

### Visual Studio project configuration

1. Open the pre-defined [solution](before/PersonManagementApp.sln) with Visual Studio 2017.

2. Create a new "Unit Test Project (.NET Framework)" with name `Spec`:

![New SpecFlow project](/doc/img/NewSpecFlowProject.png)

By default, the project uses MSTest. 

3. Get rid of the `UnitTest1.cs` that is automatically added to the project.

4. Install SpecFlow in the `Spec` project:

![Install SpecFlow](/doc/img/InstallSpecFlow.png)

5. Add a new folder `Features` to the `Spec` project

6. Create a folder `StepDefinitions` in the `Spec` project.

### Feature initialization

1. Add a new "SpecFlow Feature File" to the `Features` folder of the `Spec` project: 

![Add new SpecFlow Feature File](/doc/img/AddNewSpecFlowFeatureFile.png)

You will be provided with a feature file about a calculator adding two numbers. Just replace that specification with the above `Feature: Technical Officer manages persons`. Copy the feature with the scenarios.

2. Initially, the feature file looks like this:

![Initial Feature File](/doc/img/InitialPersonManagementFeatureFile.png)

You surely noticed that all the `Given`, `When`, `Then` steps are highlighted in purple. This means that the underlying step definition code does not exist yet. 

3. Generate the initial step definitions with a right-click on the feature file, then "Generate Step Definitions":

![Generate initial step definitions](/doc/img/GenerateInitialStepDefinitions.png)

![Step definition dialog](/doc/img/StepDefinitionGenerationDialog.png)

Store the step definition code file in the `StepDefinitions` folder of the `Spec` project. Upon step definitions generation, the step definitions are not purple any more in the feature file.

A typical generated step definition looks like this (depending on your SpecFlow version):

```
[Given(@"a list of persons was persisted to the database")]
public void GivenAListOfPersonsWasPersistedToTheDatabase()
{
  ScenarioContext.Current.Pending();
}
```

In that case, you might see the warning

![Obsolete ScenarioContext.Current](/doc/img/ObsoleteScenarioContextCurrent.png)

in which case you would just inject the `ScenarioContext` into the `TechnicalOfficerManagesPersonSteps` constructor:

```
readonly ScenarioContext _scenarioContext;

public TechnicalOfficerManagesPersonsSteps(ScenarioContext scenarioContext)
{
  _scenarioContext = scenarioContext;
}
```

and replace 

```
ScenarioContext.Current.Pending();
```

with

```
_scenarioContext.Pending();
```
