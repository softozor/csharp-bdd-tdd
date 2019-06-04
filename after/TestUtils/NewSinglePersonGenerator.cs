using FizzWare.NBuilder;
using FizzWare.NBuilder.PropertyNaming;
using Models;

namespace TestUtils
{
  /// <summary>
  /// The NewSinglePersonGenerator is a generator of new persons based on the NBuilder 3rd party library. 
  /// It is only used to build new persons in our tests. The persons are generated with invalid id, not random id, but with random fields.
  /// Such persons complete well an already built sequential list of persons (with auto-incremented person id).
  /// </summary>
  public class NewSinglePersonGenerator
  {
    /// <summary>Sets up NBulider with RandomValuePropertyNamer.</summary>
    public NewSinglePersonGenerator()
    {
      BuilderSetup.SetDefaultPropertyName(new RandomValuePropertyNamer(new BuilderSettings()));
    }

    /// <summary>Generates a <see cref="Person"/> with random data and invalid id.</summary>
    /// <returns></returns>
    public Person Generate()
    {
      return Builder<Person>.CreateNew().WithInvalidId().Build();
    }
  }
}
