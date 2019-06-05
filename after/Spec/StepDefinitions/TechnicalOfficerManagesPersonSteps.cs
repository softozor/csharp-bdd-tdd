using DataAccess.Handlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using PersonManagementModule.Services;
using System.Linq;
using TechTalk.SpecFlow;

namespace PersonManagementSpec.StepDefinitions
{
  [Binding]
  public class TechnicalOfficerManagesPersonSteps
  {
    const string FILE_TO_IMPORT = "Fixtures/PersonsToImport.json";

    readonly ScenarioContext _scenarioContext;
    readonly IPersonProvider _personProvider;
    readonly PersonManager _personManager;

    public TechnicalOfficerManagesPersonSteps(PersonManager personManager, IPersonProvider personProvider, ScenarioContext scenarioContext)
    {
      _personManager = personManager;
      _personProvider = personProvider;
      _scenarioContext = scenarioContext;
    }

    [Given(@"a list of persons was persisted to the database")]
    public void GivenAListOfPersonsWasPersistedToTheDatabase()
    {
      var persons = _personProvider.GetPersons();
      Assert.IsTrue(persons.Count() > 0);
    }

    [Given(@"the Technical Officer has added a new person")]
    public void GivenTheTechnicalOfficerHasAddedANewPerson()
    {
      var person = _personManager.AddNewPerson();
      _scenarioContext.Set(person);
    }

    [When(@"she saves(?:.*)")]
    public void WhenSheSaves()
    {
      _personManager.Save();
    }

    [When(@"the Technical Officer imports a list of persons")]
    public void WhenTheTechnicalOfficerImportsAListOfPersons()
    {
      ImportNonEmptyListOfPersons();
    }

    private void ImportNonEmptyListOfPersons()
    {
      _scenarioContext.Set(_personProvider.GetPersons().Count(), "initialPersonsCount");
      _personManager.Import(FILE_TO_IMPORT);
      var personsToImport = JsonFileHandler.ReadPersons(FILE_TO_IMPORT);
      Assert.IsTrue(personsToImport.Count() > 0);
    }

    [Then(@"the new person is persisted to the database")]
    public void ThenTheNewPersonIsPersistedToTheDatabase()
    {
      var newPerson = _scenarioContext.Get<Person>();
      var persistedPersons = _personProvider.GetPersons();
      var isNewPersonPersistedToDatabase = persistedPersons.Any(
        person => person.FirstName == newPerson.FirstName
          && person.LastName == newPerson.LastName
          && person.Title == newPerson.Title
      );
      Assert.IsTrue(isNewPersonPersistedToDatabase);
    }

    [Then(@"they are not persisted to the database")]
    public void ThenTheyAreNotPersistedToTheDatabase()
    {
      var expectedPersonsCount = _scenarioContext.Get<int>("initialPersonsCount");
      var actualPersonsCount = _personProvider.GetPersons().Count();
      Assert.AreEqual(expectedPersonsCount, actualPersonsCount);
    }
  }
}
