using FunctionalExtensions.Base;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Lenses.Implementations;

namespace Janus.Lenses.Tests.Implementations;
public class TabularDataDtoLensTests : BaseLensTesting
{
    private readonly TabularData _peopleTabularData =
        TabularDataBuilder.InitTabularData(new Dictionary<string, Commons.SchemaModels.DataTypes>
        {
            { "Id", DataTypes.LONGINT },
            { "FirstName", DataTypes.STRING },
            { "LastName", DataTypes.STRING },
            { "DateOfBirth", DataTypes.DATETIME }
        })
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1001L },
            { "FirstName", "John" },
            { "LastName", "Smith" },
            { "DateOfBirth", new DateTime(1985, 7, 14)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1002L },
            { "FirstName", "Emily" },
            { "LastName", "Johnson" },
            { "DateOfBirth", new DateTime(1992, 2, 19)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1003L },
            { "FirstName", "Michael" },
            { "LastName", "Davis" },
            { "DateOfBirth", new DateTime(1978, 11, 2)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1004L },
            { "FirstName", "Sarah" },
            { "LastName", "Lee" },
            { "DateOfBirth", new DateTime(1989, 6, 28)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1005L },
            { "FirstName", "James" },
            { "LastName", "Brown" },
            { "DateOfBirth", new DateTime(1965, 9, 9)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1006L },
            { "FirstName", "Jennifer" },
            { "LastName", "Rodriguez" },
            { "DateOfBirth", new DateTime(1996, 4, 23)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1007L },
            { "FirstName", "David" },
            { "LastName", "Garcia" },
            { "DateOfBirth", new DateTime(1983, 1, 11)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1008L },
            { "FirstName", "Lisa" },
            { "LastName", "Wilson" },
            { "DateOfBirth", new DateTime(1974, 8, 6)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1009L },
            { "FirstName", "Robert" },
            { "LastName", "Martinez" },
            { "DateOfBirth", new DateTime(1980, 3, 17)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1010L },
            { "FirstName", "Jessica" },
            { "LastName", "Anderson" },
            { "DateOfBirth", new DateTime(1999, 12, 30)}
        }))
        .Build();

    public override void GetPutTest()
    {
        var lens = TabularDataDtoLenses.Construct<PersonDto>();

        var source = _peopleTabularData;

        // put (get s) s = s
        var view = lens.Get(source);
        var sourceOverOriginal = lens.Put(view, source); // original source is provided
        var sourceOverNull = lens.Put(view, null); // no source is provided

        Assert.Equal(source, sourceOverOriginal);
        Assert.NotEqual(source.Name, sourceOverNull.Name); // new name is generated - as expected
        Assert.Equal(source.RowData, sourceOverNull.RowData);
    }

    public override void PutGetTest()
    {
        var lens = TabularDataDtoLenses.Construct<PersonDto>();

        var source = _peopleTabularData;
        var expectedView =
            _peopleTabularData.RowData.Map(rowData =>
                new PersonDto
                {
                    Id = (long)rowData["Id"],
                    FirstName = (string)rowData["FirstName"],
                    LastName = (string)rowData["LastName"],
                    DateOfBirth = (DateTime)rowData["DateOfBirth"]
                });

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
        var lens = TabularDataDtoLenses.Construct<PersonDto>();

        var originalView = _peopleTabularData.RowData.Map(rowData =>
            new PersonDto
            {
                Id = (long)rowData["Id"],
                FirstName = (string)rowData["FirstName"],
                LastName = (string)rowData["LastName"],
                DateOfBirth = (DateTime)rowData["DateOfBirth"]
            });

        var changedView = _peopleTabularData.RowData.Mapi((idx, rowData) =>
            new PersonDto
            {
                Id = (long)rowData["Id"],
                FirstName = (idx % 2 == 0 ? "x_" : string.Empty) + (string)rowData["FirstName"],
                LastName = (string)rowData["LastName"],
                DateOfBirth = idx % 2 != 0 ? (DateTime)rowData["DateOfBirth"] : ((DateTime)rowData["DateOfBirth"]).AddYears(2)
            });


        var source = _peopleTabularData;

        // put v' (put v s) = put v' s
        var returnedSource = lens.Put(originalView, source);
        var returnedNullSource = lens.Put(originalView, null);

        var returnedSourceOverUpdate = lens.Put(changedView, returnedSource);
        var returnedNullSourceOverUpdate = lens.Put(changedView, returnedNullSource);

        var directlyReturnedSourceOverUpdate = lens.Put(changedView, source);

        Assert.Equal(returnedSourceOverUpdate, directlyReturnedSourceOverUpdate);
        Assert.NotEqual(returnedNullSourceOverUpdate.Name, directlyReturnedSourceOverUpdate.Name); // new name generated - as expected
        Assert.Equal(returnedNullSourceOverUpdate.RowData, directlyReturnedSourceOverUpdate.RowData);
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
