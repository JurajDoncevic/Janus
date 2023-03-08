using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Lenses.Implementations;

namespace Janus.Lenses.Tests.Implementations;
public class RowDataDtoLensTests : BaseLensTesting
{

    private readonly TabularData _peopleTabularData =
        TabularDataBuilder.InitTabularData(new Dictionary<string, Commons.SchemaModels.DataTypes>
        {
                { "Id", DataTypes.LONGINT },
                { "FirstName", DataTypes.STRING },
                { "LastName", DataTypes.STRING },
                { "DateOfBirth", DataTypes.DATETIME }
        }).AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
                { "Id", 1L },
                { "FirstName", "John" },
                { "LastName", "Doe" },
                { "DateOfBirth", new DateTime(1965, 1, 1)}
        })).AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
                { "Id", 2L },
                { "FirstName", "James" },
                { "LastName", "Smith" },
                { "DateOfBirth", new DateTime(1966, 1, 1)}
        }))
        .Build();

    public override void GetPutTest()
    {
        var lens = RowDataDtoLenses.Construct<PersonDto>();

        var source = _peopleTabularData.RowData.First();

        // put (get s) s = s
        var view = lens.Get(source);
        var sourceOverOriginal = lens.Put(view, source); // original source is provided
        var sourceOverNull = lens.Put(view, null); // no source is provided

        Assert.Equal(source, sourceOverOriginal);
        Assert.Equal(source, sourceOverNull);
    }

    public override void PutGetTest()
    {
        var lens = RowDataDtoLenses.Construct<PersonDto>();

        var source = _peopleTabularData.RowData.First();
        var expectedView = new PersonDto
        {
            Id = (long)_peopleTabularData[0]["Id"],
            FirstName = (string)_peopleTabularData[0]["FirstName"],
            LastName = (string)_peopleTabularData[0]["LastName"],
            DateOfBirth = (DateTime)_peopleTabularData[0]["DateOfBirth"]
        };

        // get(put v s) = v
        var sourceOverOriginal = lens.Put(expectedView, source);
        var sourceOverNull = lens.Put(expectedView, null);

        var viewOverOriginal = lens.Get(sourceOverOriginal);
        var viewOverNull = lens.Get(sourceOverNull);

        Assert.Equal(expectedView, viewOverOriginal);
        Assert.Equal(expectedView, viewOverNull);
    }

    public override void PutPutTest()
    {
        var lens = RowDataDtoLenses.Construct<PersonDto>();

        var originalView = new PersonDto
        {
            Id = (long)_peopleTabularData[0]["Id"],
            FirstName = (string)_peopleTabularData[0]["FirstName"],
            LastName = (string)_peopleTabularData[0]["LastName"],
            DateOfBirth = (DateTime)_peopleTabularData[0]["DateOfBirth"]
        };

        var changedView = new PersonDto
        {
            Id = (long)_peopleTabularData[1]["Id"],
            FirstName = (string)_peopleTabularData[1]["FirstName"],
            LastName = (string)_peopleTabularData[1]["LastName"],
            DateOfBirth = (DateTime)_peopleTabularData[1]["DateOfBirth"]
        };

        var source = _peopleTabularData[0];

        // put v' (put v s) = put v' s
        var returnedSource = lens.Put(originalView, source);
        var returnedNullSource = lens.Put(originalView, null);

        var returnedSourceOverUpdate = lens.Put(changedView, returnedSource);
        var returnedNullSourceOverUpdate = lens.Put(changedView, returnedNullSource);

        var directlyReturnedSourceOverUpdate = lens.Put(changedView, source);

        Assert.Equal(returnedSourceOverUpdate, directlyReturnedSourceOverUpdate);
        Assert.Equal(returnedNullSourceOverUpdate, directlyReturnedSourceOverUpdate);
    }

    private class PersonDto
    {
        private long _Id;
        private string _FirstName;
        private string _LastName;
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
    }
}
