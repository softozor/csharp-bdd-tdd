using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using PersonManagementModule.Services;
using PersonManagementModule.ViewModels;
using System.Linq;
using TestUtils;

namespace PersonManagementTests
{
  [TestClass]
  public class ViewModelTests
  {
    [TestInitialize]
    public void ResetTestDataBuilderToDefault()
    {
      BuilderSetup.ResetToDefaults();
    }

    [TestMethod]
    public void ShouldDisplayPersistedPersons()
    {
      var personProviderMock = new Mock<IPersonProvider>();
      personProviderMock.Setup(provider => provider.GetPersons())
        .Returns(Builder<Person>.CreateListOfSize(10).Build());
      var viewModel = new PersonViewModel(personProviderMock.Object);

      var personsInDatabase = personProviderMock.Object.GetPersons();
      var personsInViewModel = from person in viewModel.Persons select person.Model;

      CollectionAssert.AreEqual(personsInDatabase.ToList(), personsInViewModel.ToList());
    }

    [TestMethod]
    public void ShouldPersistNewPerson()
    {
      var personProviderMock = new Mock<IPersonProvider>();
      personProviderMock.Setup(provider => provider.GetPersons())
        .Returns(Builder<Person>.CreateListOfSize(10).Build());
      var viewModel = new PersonViewModel(personProviderMock.Object);

      var personGenerator = new NewSinglePersonGenerator();
      var model = personGenerator.Generate();
      var personItem = new PersonItem(model);
      viewModel.Persons.Add(personItem);
      viewModel.SavePersonsCommand.Execute();

      var personsToBeSaved = from item in viewModel.Persons select item.Model;
      personProviderMock.Verify(provider => provider.Save(personsToBeSaved), Times.Once());
    }

    // TODO: if there are no changes in the PersonItems, then the save should not be possible
  }
}
