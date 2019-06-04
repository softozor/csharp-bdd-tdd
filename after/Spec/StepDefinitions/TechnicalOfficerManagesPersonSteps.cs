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

    [Given(@"the Technical Officer imported a list of persons")]
    public void GivenTheTechnicalOfficerImportedAListOfPersons()
    {
      _scenarioContext.Pending();
    }

    [Given(@"the Technical Officer has selected a few persons")]
    public void GivenTheTechnicalOfficerHasSelectedAFewPersons()
    {
      _scenarioContext.Pending();
    }

    [Given(@"the Technical Officer has selected a person")]
    public void GivenTheTechnicalOfficerHasSelectedAPerson()
    {
      _scenarioContext.Pending();
    }

    [Given(@"the Technical Officer has deleted a person")]
    public void GivenTheTechnicalOfficerHasDeletedAPerson()
    {
      _scenarioContext.Pending();
    }

    [Given(@"the Technical Officer has deleted a few persons")]
    public void GivenTheTechnicalOfficerHasDeletedAFewPersons()
    {
      _scenarioContext.Pending();
    }

    [When(@"she saves(?:.*)")]
    public void WhenSheSaves()
    {
      _personManager.Save();
    }

    [When(@"the Technical Officer imports a list of persons")]
    public void WhenTheTechnicalOfficerImportsAListOfPersons()
    {
      _scenarioContext.Pending();
    }

    [When(@"she exports all the persons to a local file")]
    public void WhenSheExportsAllThePersonsToALocalFile()
    {
      _scenarioContext.Pending();
    }

    [When(@"she exports them to a local file")]
    public void WhenSheExportsThemToALocalFile()
    {
      _scenarioContext.Pending();
    }

    [When(@"she deletes her")]
    public void WhenSheDeletesHer()
    {
      _scenarioContext.Pending();
    }

    [When(@"she deletes them")]
    public void WhenSheDeletesThem()
    {
      _scenarioContext.Pending();
    }

    [When(@"she prints badges for everyone in the list")]
    public void WhenShePrintsBadgesForEveryoneInTheList()
    {
      _scenarioContext.Pending();
    }

    [When(@"the Technical Officer selects a few persons")]
    public void WhenTheTechnicalOfficerSelectsAFewPersons()
    {
      _scenarioContext.Pending();
    }

    [When(@"prints badges for them")]
    public void WhenPrintsBadgesForThem()
    {
      _scenarioContext.Pending();
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

    [Then(@"the new persons can be seen in the view")]
    public void ThenTheNewPersonsCanBeSeenInTheView()
    {
      _scenarioContext.Pending();
    }

    [Then(@"they are not persisted to the database")]
    public void ThenTheyAreNotPersistedToTheDatabase()
    {
      _scenarioContext.Pending();
    }

    [Then(@"the new persons are added to the database")]
    public void ThenTheNewPersonsAreAddedToTheDatabase()
    {
      _scenarioContext.Pending();
    }

    [Then(@"the local file is filled up with the details of all the persons depicted in the view")]
    public void ThenTheLocalFileIsFilledUpWithTheDetailsOfAllThePersonsDepictedInTheView()
    {
      _scenarioContext.Pending();
    }

    [Then(@"the local file is filled up with the selected persons' details")]
    public void ThenTheLocalFileIsFilledUpWithTheSelectedPersonsDetails()
    {
      _scenarioContext.Pending();
    }

    [Then(@"the person disappears from the view")]
    public void ThenThePersonDisappearsFromTheView()
    {
      _scenarioContext.Pending();
    }

    [Then(@"it is still present in the database")]
    public void ThenItIsStillPresentInTheDatabase()
    {
      _scenarioContext.Pending();
    }

    [Then(@"the person is not in the database anymore")]
    public void ThenThePersonIsNotInTheDatabaseAnymore()
    {
      _scenarioContext.Pending();
    }

    [Then(@"the persons disappear from the view")]
    public void ThenThePersonsDisappearFromTheView()
    {
      _scenarioContext.Pending();
    }

    [Then(@"they are still present in the database")]
    public void ThenTheyAreStillPresentInTheDatabase()
    {
      _scenarioContext.Pending();
    }

    [Then(@"the persons are not in the database anymore")]
    public void ThenThePersonsAreNotInTheDatabaseAnymore()
    {
      _scenarioContext.Pending();
    }

    [Then(@"she gets a PDF file with all the badges")]
    public void ThenSheGetsAPDFFileWithAllTheBadges()
    {
      _scenarioContext.Pending();
    }

    [Then(@"she gets a PDF file with the badges of the selected persons")]
    public void ThenSheGetsAPDFFileWithTheBadgesOfTheSelectedPersons()
    {
      _scenarioContext.Pending();
    }
  }
}
