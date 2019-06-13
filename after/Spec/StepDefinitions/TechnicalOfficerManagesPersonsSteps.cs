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

    [When(@"the Technical Officer imports a list of persons")]
    public void WhenTheTechnicalOfficerImportsAListOfPersons()
    {
      _scenarioContext.Pending();
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

    [Then(@"she has access to the persisted persons")]
    public void ThenSheHasAccessToThePersistedPersons()
    {
      var persistedPersons = _dataService.GetAllPersons().OrderBy(person => person.Id);
      var accessiblePersons = _personManager.GetAccessiblePersons().OrderBy(person => person.Id);
      CollectionAssert.AreEqual(persistedPersons.ToList(), accessiblePersons.ToList());
    }

    [Then(@"they are not persisted to the database")]
    public void ThenTheyAreNotPersistedToTheDatabase()
    {
      _scenarioContext.Pending();
    }
  }
}
