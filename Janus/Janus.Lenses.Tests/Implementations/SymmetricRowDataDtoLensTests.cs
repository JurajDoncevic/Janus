using FunctionalExtensions.Base;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Lenses.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Lenses.Tests.Implementations;
public class SymmetricRowDataDtoLensTests : BaseSymmetricLensTests
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

    public override void CreatePutLRTest()
    {
        throw new NotImplementedException();
    }

    public override void CreatePutRLTest()
    {
        var lens = SymmetricRowDataDtoLenses.Construct<PersonDto>();

        var left = _peopleTabularData.RowData.First();

        var result = lens.PutLeft(lens.CreateRight(Option<RowData>.Some(left)), left);

        Assert.Equal(left, result);
    }

    public override void PutLRTest()
    {
        throw new NotImplementedException();
    }

    public override void PutRLTest()
    {
        throw new NotImplementedException();
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
