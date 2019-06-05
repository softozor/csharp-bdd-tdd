using DataAccess.Handlers;
using FizzWare.NBuilder;
using FizzWare.NBuilder.PropertyNaming;
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
    Mock<IPersonProvider> _personProviderMock;
    Mock<IFileHandlerFactory> _fileHandlerFactoryMock;
    PersonViewModel _viewModel;

    [TestInitialize]
    public void Init()
    {
      InitializeViewModel();
      ResetTestDataBuilderToDefault();
    }

    private void InitializeViewModel()
    {
      _personProviderMock = new Mock<IPersonProvider>();
      _personProviderMock.Setup(provider => provider.GetPersons())
        .Returns(Builder<Person>.CreateListOfSize(10).Build());
      _fileHandlerFactoryMock = new Mock<IFileHandlerFactory>();
      _viewModel = new PersonViewModel(_fileHandlerFactoryMock.Object, _personProviderMock.Object);
    }

    private void ResetTestDataBuilderToDefault()
    {
      BuilderSetup.ResetToDefaults();
    }

    /// <summary>
    /// Upon loading, the <see cref="PersonViewModel"/> needs to have the persisted persons listed 
    /// in its <see cref="PersonViewModel.Persons">observable collection</see>.
    /// </summary>
    [TestMethod]
    public void ShouldDisplayPersistedPersons()
    {
      var personsInDatabase = _personProviderMock.Object.GetPersons();
      var personsInViewModel = from person in _viewModel.Persons select person.Model;

      CollectionAssert.AreEqual(personsInDatabase.ToList(), personsInViewModel.ToList());
    }


    /// <summary>
    /// Upon saving of a new person, that new person needs to be persisted to the underlying database.
    /// </summary>
    [TestMethod]
    public void ShouldPersistNewPerson()
    {
      var personGenerator = new NewSinglePersonGenerator();
      var model = personGenerator.Generate();
      var personItem = new PersonItem(model);
      _viewModel.Persons.Add(personItem);
      _viewModel.SavePersonsCommand.Execute();

      var personsToBeSaved = from item in _viewModel.Persons select item.Model;
      _personProviderMock.Verify(provider => provider.Save(personsToBeSaved), Times.Once());
    }

    /// <summary>
    /// Upon importation of a persons' list, the new persons are added to the <see cref="PersonViewModel"/>'s
    /// <see cref="PersonViewModel.Persons">observable collection</see> with invalid IDs.
    /// </summary>
    [TestMethod]
    public void ShouldAddImportedPersonsToObservableCollectionWithInvalidIds()
    {
      // Arrange
      var listOfNewPersons = Enumerable.Empty<Person>();
      var fileHandlerMock = new Mock<IFileHandler>();
      fileHandlerMock.Setup(handler => handler.ReadFile())
        .Returns(Builder<Person>.CreateListOfSize(10, new RandomValuePropertyNamer(new BuilderSettings())).WithInvalidId().Build());
      _fileHandlerFactoryMock.Setup(factory => factory.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
        .Returns(fileHandlerMock.Object);

      _viewModel.Persons.CollectionChanged += (s, e) =>
      {
        var newPersonItem = e.NewItems[0] as PersonItem;
        listOfNewPersons = listOfNewPersons.Append(newPersonItem.Model);
      };
      var persons = fileHandlerMock.Object.ReadFile();

      // Act
      var payload = new ImportPayload
      {
        Filename = "MyRandomFilename.json",
        FileType = "Json"
      };
      _viewModel.ImportPersonsCommand.Execute(payload);

      // Assert
      var setDifference = persons.Except(listOfNewPersons, (person1, person2) =>
      {
        return person1.FirstName == person2.FirstName
          && person1.LastName == person2.LastName
          && person1.Title == person2.Title;
      });
      Assert.AreEqual(0, setDifference.Count());
      Assert.IsTrue(listOfNewPersons.All(person => !person.Id.HasValue));
    }
  }
}
