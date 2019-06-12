using Microsoft.VisualStudio.TestTools.UnitTesting;
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
  }
}
