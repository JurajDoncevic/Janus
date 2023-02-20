using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Lenses.Tests;
public class LensTests
{

    [Fact]
    public void TestRowDataLens()
    {
        var lens = RowDataDtoObjectLens.Create<PersonDto>();

        var dto = lens.Get(PeopleData1.RowData.First());
        var rowData = lens.Put(dto, PeopleData1.RowData.First());

        Assert.NotNull(dto);
        Assert.NotNull(rowData);
    }


    private readonly TabularData PeopleData1 =
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
    }
}
