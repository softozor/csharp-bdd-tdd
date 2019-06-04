using System;

namespace Models
{
  public class Person : IEquatable<Person>
  {
    public int? Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Title { get; set; }

    public override bool Equals(object obj)
    {
      return Equals((Person)obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

    public override string ToString()
    {
      return $"Person {Id.ToString()} is {Title} {FirstName} {LastName}";
    }

    public bool Equals(Person other)
    {
      if (other == null)
      {
        return false;
      }
      return Id == other.Id
        && FirstName == other.FirstName
        && LastName == other.LastName
        && Title == other.Title;
    }
  }
}
