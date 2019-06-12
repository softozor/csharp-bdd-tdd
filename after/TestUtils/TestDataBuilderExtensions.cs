using FizzWare.NBuilder;
using Models;

namespace TestUtils
{
  public static class TestDataBuilderExtensions
  {
    public static ISingleObjectBuilder<Person> WithInvalidId(this ISingleObjectBuilder<Person> builder)
    {
      return builder.With(person => person.Id = null);
    }
  }
}
