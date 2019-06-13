using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using PersonManagementModule.Services;
using PersonManagementModule.ViewModels;
using System.Linq;
using TestUtils;

namespace Tests
{
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
      _personProviderMock.Setup(provider => provider.GetPersons())
        .Returns(Builder<Person>.CreateListOfSize(10).Build());
      _viewModel = new PersonViewModel(_personProviderMock.Object);
    }

    /// <summary>
    /// Upon saving of a new person, that new person needs to be persisted to the underlying database.
    /// </summary>
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

    /// <summary>
    /// Upon <see cref="PersonViewModel"/> construction, the persisted data need to be loaded in the <see cref="PersonViewModel.Persons"/> collection.
    /// </summary>
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
  }
}
