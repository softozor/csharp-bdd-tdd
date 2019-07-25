using DataAccess.Services;
using FizzWare.NBuilder;
using FizzWare.NBuilder.PropertyNaming;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using PersonManagementModule.Services;
using PersonManagementModule.ViewModels;
using System;
using System.Linq;

namespace Tests
{
  [TestClass]
  public class PersonProviderTests
  {
    Mock<IDataService> _dataServiceMock;
    PersonProvider _provider;

    [TestInitialize]
    public void Initialize()
    {
      _dataServiceMock = new Mock<IDataService>();
      _provider = new PersonProvider(_dataServiceMock.Object);

      SetupDataBuilder();
    }

    private void SetupDataBuilder()
    {
      BuilderSetup.SetDefaultPropertyName(new RandomValuePropertyNamer(new BuilderSettings()));
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void SaveThrowsIfNullIsProvided()
    {
      // When
      _provider.Save(null);

      // Then: the assertions are triggered by the ExpectedException attribute
    }

    [TestMethod]
    public void ShouldPersistPersonsUponSaving()
    {
      // When
      var models = Builder<Person>.CreateListOfSize(10).Build();
      var persons = from model in models select new PersonItem(model);
      _provider.Save(persons);

      // Then
      _dataServiceMock.Verify(service => service.SavePersons(models), Times.Once());
    }
  }
}
