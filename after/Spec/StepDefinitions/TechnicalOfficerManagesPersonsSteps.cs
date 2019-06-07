using System;
using TechTalk.SpecFlow;

namespace Spec.StepDefinitions
{
  [Binding]
  public class TechnicalOfficerManagesPersonsSteps
  {
    readonly ScenarioContext _scenarioContext;

    public TechnicalOfficerManagesPersonsSteps(ScenarioContext scenarioContext)
    {
      _scenarioContext = scenarioContext;
    }

    [Given(@"a list of persons was persisted to the database")]
    public void GivenAListOfPersonsWasPersistedToTheDatabase()
    {
      _scenarioContext.Pending();
    }

    [Given(@"the Technical Officer has added a new person")]
    public void GivenTheTechnicalOfficerHasAddedANewPerson()
    {
      _scenarioContext.Pending();
    }

    [When(@"she saves")]
    public void WhenSheSaves()
    {
      _scenarioContext.Pending();
    }

    [When(@"the Technical Officer imports a list of persons")]
    public void WhenTheTechnicalOfficerImportsAListOfPersons()
    {
      _scenarioContext.Pending();
    }

    [Then(@"the new person is persisted to the database")]
    public void ThenTheNewPersonIsPersistedToTheDatabase()
    {
      _scenarioContext.Pending();
    }

    [Then(@"they are not persisted to the database")]
    public void ThenTheyAreNotPersistedToTheDatabase()
    {
      _scenarioContext.Pending();
    }
  }
}
