using FizzWare.NBuilder;
using Models;

namespace TestUtils
{
  public static class TestDataBuilderExtensions
  {
    public static ISingleObjectBuilder<Person> WithInvalidId(this ISingleObjectBuilder<Person> objectBuilder)
    {
      return objectBuilder.With(person => person.Id = -1);
    }
  }
}
