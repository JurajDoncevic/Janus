namespace Janus.Lenses.Tests.Implementations.TestingClasses;
public class PersonDto
{
    private long _Id;
    private string _FirstName = string.Empty;
    private string _LastName = string.Empty;
    private DateTime _DateOfBirth;

    public long Id { get => _Id; set => _Id = value; }
    public string FirstName { get => _FirstName; set => _FirstName = value; }
    public string LastName { get => _LastName; set => _LastName = value; }
    public DateTime DateOfBirth { get => _DateOfBirth; set => _DateOfBirth = value; }

    public override bool Equals(object? obj)
    {
        return obj is PersonDto dto &&
               _Id.Equals(dto._Id) &&
               _FirstName.Equals(dto._FirstName) &&
               _LastName.Equals(dto._LastName) &&
               _DateOfBirth.Equals(dto._DateOfBirth);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_Id, _FirstName, _LastName, _DateOfBirth);
    }
}