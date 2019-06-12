using FizzWare.NBuilder;
using FizzWare.NBuilder.PropertyNaming;
using Models;

namespace TestUtils
{
  public class NewSinglePersonGenerator
  {
    public NewSinglePersonGenerator()
    {
      BuilderSetup.SetDefaultPropertyName(new RandomValuePropertyNamer(new BuilderSettings()));
    }

    public Person Generate()
    {
      return Builder<Person>.CreateNew().WithInvalidId().Build();
    }
  }
}
