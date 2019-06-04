using DataAccess;
using FizzWare.NBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Moq;
using PersonManagementModule.Services;
using System;
using System.Linq;

namespace PersonManagementTests
{
  [TestClass]
  public class PersonProviderTests
  {
    [TestMethod]
    public void ShouldGetPersistedPersons()
    {
      var dataServiceMock = new Mock<IDataService>();
      dataServiceMock.Setup(service => service.GetAllPersons())
        .Returns(Builder<Person>.CreateListOfSize(10).Build());
      var provider = new PersonProvider(dataServiceMock.Object);

      System.Func<Person, int?> personId = person => person.Id;
      var personsInDatabase = dataServiceMock.Object.GetAllPersons().OrderBy(personId);
      var personsInProvider = provider.GetPersons().OrderBy(personId);

      CollectionAssert.AreEqual(personsInDatabase.ToList(), personsInProvider.ToList());
    }

    [TestMethod]
    [ExpectedException(typeof(NullReferenceException))]
    public void SaveThrowsIfNullIsProvided()
    {
      var dataServiceMock = new Mock<IDataService>();
      var provider = new PersonProvider(dataServiceMock.Object);
      provider.Save(null);
    }
  }
}
