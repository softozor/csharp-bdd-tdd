using DataAccess.Handlers;
using FizzWare.NBuilder;
using FizzWare.NBuilder.PropertyNaming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using PersonManagementModule.Services;
using PersonManagementModule.ViewModels;
using System.Linq;

namespace Tests
{
  [TestClass]
  public class ViewModelTests
  {
    Mock<IFileHandlerFactory> _fileHandlerFactoryMock;
    Mock<IPersonProvider> _personProviderMock;
    PersonViewModel _viewModel;

    [TestInitialize]
    public void Init()
    {
      SetupDataBuilder();
      InitializeViewModel();
    }

    private void SetupDataBuilder()
    {
      BuilderSetup.SetDefaultPropertyName(new RandomValuePropertyNamer(new BuilderSettings()));
    }

    private void InitializeViewModel()
    {
      _personProviderMock = new Mock<IPersonProvider>();
      var persistedModels = Builder<Person>.CreateListOfSize(10).Build();
      var persistedPersons = from model in persistedModels select new PersonItem(model);
      _personProviderMock.Setup(provider => provider.GetPersons())
        .Returns(persistedPersons);
      _fileHandlerFactoryMock = new Mock<IFileHandlerFactory>();
      _viewModel = new PersonViewModel(_fileHandlerFactoryMock.Object, _personProviderMock.Object);
    }

    /// <summary>
    /// Upon saving of a new person, that new person needs to be persisted to the underlying database.
    /// </summary>
    [TestMethod]
    public void ShouldPersistNewPerson()
    {
      // Given
      var model = Builder<Person>.CreateNew().Build();
      var personItem = new PersonItem(model);

      // When
      _viewModel.Persons.Add(personItem);
      _viewModel.SavePersonsCommand.Execute();

      // Then
      _personProviderMock.Verify(provider => provider.Save(_viewModel.Persons), Times.Once());
    }

    /// <summary>
    /// Upon <see cref="PersonViewModel"/> construction, the persisted data need to be loaded in the <see cref="PersonViewModel.Persons"/> collection.
    /// </summary>
    [TestMethod]
    public void ShouldDisplayPersistedPersons()
    {
      // Given
      var persistedPersons = _personProviderMock.Object.GetPersons();
      var modelsInDatabase = from person in persistedPersons select person.Model;

      // When
      var accessiblePersons = from personItem in _viewModel.Persons select personItem.Model;

      // Then
      CollectionAssert.AreEqual(modelsInDatabase.ToList(), accessiblePersons.ToList());
    }

    /// <summary>
    /// Upon importation of a persons' list, the new persons are added to the <see cref="PersonViewModel"/>'s
    /// <see cref="PersonViewModel.Persons">observable collection</see> with invalid IDs.
    /// </summary>
    [TestMethod]
    public void ShouldAddImportedPersonsToObservableCollection()
    {
      // Given
      var fileHandlerMock = new Mock<IFileHandler>();
      fileHandlerMock.Setup(handler => handler.ReadFile())
        .Returns(Builder<Person>.CreateListOfSize(10).Build());
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
  }
}
