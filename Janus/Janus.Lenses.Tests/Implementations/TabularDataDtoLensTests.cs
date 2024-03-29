﻿using Janus.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.SchemaModels;
using Janus.Lenses.Implementations;
using Janus.Lenses.Tests.Implementations.TestingClasses;

namespace Janus.Lenses.Tests.Implementations;
public sealed class TabularDataDtoLensTests : SymmetricLensTestingFramework<TabularData, IEnumerable<PersonDto>>
{
    protected override TabularData _x =>
        TabularDataBuilder.InitTabularData(new Dictionary<string, Commons.SchemaModels.DataTypes>
        {
            { "Id", DataTypes.LONGINT },
            { "Coefficient", DataTypes.DECIMAL },
            { "FirstName", DataTypes.STRING },
            { "LastName", DataTypes.STRING },
            { "DateOfBirth", DataTypes.DATETIME }
        })
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1001L },
            {"Coefficient", 1.0 },
            { "FirstName", "John" },
            { "LastName", "Smith" },
            { "DateOfBirth", new DateTime(1985, 7, 14)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1002L },
            {"Coefficient", 1.1 },
            { "FirstName", "Emily" },
            { "LastName", "Johnson" },
            { "DateOfBirth", new DateTime(1992, 2, 19)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1003L },
            {"Coefficient", 1.2 },
            { "FirstName", "Michael" },
            { "LastName", "Davis" },
            { "DateOfBirth", new DateTime(1978, 11, 2)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1004L },
            { "Coefficient", 1.3 },
            { "FirstName", "Sarah" },
            { "LastName", "Lee" },
            { "DateOfBirth", new DateTime(1989, 6, 28)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1005L },
            { "Coefficient", 1.4 },
            { "FirstName", "James" },
            { "LastName", "Brown" },
            { "DateOfBirth", new DateTime(1965, 9, 9)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1006L },
            { "Coefficient", 1.5 },
            { "FirstName", "Jennifer" },
            { "LastName", "Rodriguez" },
            { "DateOfBirth", new DateTime(1996, 4, 23)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1007L },
            { "Coefficient", 1.6 },
            { "FirstName", "David" },
            { "LastName", "Garcia" },
            { "DateOfBirth", new DateTime(1983, 1, 11)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1008L },
            {"Coefficient", 1.7 },
            { "FirstName", "Lisa" },
            { "LastName", "Wilson" },
            { "DateOfBirth", new DateTime(1974, 8, 6)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1009L },
            {"Coefficient", -1.0 },
            { "FirstName", "Robert" },
            { "LastName", "Martinez" },
            { "DateOfBirth", new DateTime(1980, 3, 17)}
        }))
        .AddRow(conf => conf.WithRowData(new Dictionary<string, object?>()
        {
            { "Id", 1010L },
            {"Coefficient", -1.1 },
            { "FirstName", "Jessica" },
            { "LastName", "Anderson" },
            { "DateOfBirth", new DateTime(1999, 12, 30)}
        }))
        .WithName("PeopleData")
        .Build();

    protected override IEnumerable<PersonDto> _y => new List<PersonDto>
    {
        new PersonDto
        {
            Id = 1001L,
            Coefficient = 1.0,
            FirstName = "John",
            LastName = "Smith",
            DateOfBirth = new DateTime(1985, 7, 14)
        },
        new PersonDto
        {
            Id = 1002L,
            Coefficient = 1.1,
            FirstName = "Emily",
            LastName = "Johnson",
            DateOfBirth = new DateTime(1992, 2, 19)
        },
        new PersonDto
        {
            Id = 1003L,
            Coefficient = 1.2,
            FirstName = "Michael",
            LastName = "Davis",
            DateOfBirth = new DateTime(1978, 11, 2)
        },
        new PersonDto
        {
            Id = 1004L,
            Coefficient = 1.3,
            FirstName = "Sarah",
            LastName = "Lee",
            DateOfBirth = new DateTime(1989, 6, 28)
        },
        new PersonDto
        {
            Id = 1005L,
            Coefficient = 1.4,
            FirstName = "James",
            LastName = "Brown",
            DateOfBirth = new DateTime(1965, 9, 9)
        },
        new PersonDto
        {
            Id = 1006L,
            Coefficient = 1.5,
            FirstName = "Jennifer",
            LastName = "Rodriguez",
            DateOfBirth = new DateTime(1996, 4, 23)
        },
        new PersonDto
        {
            Id = 1007L,
            Coefficient = 1.6,
            FirstName = "David",
            LastName = "Garcia",
            DateOfBirth = new DateTime(1983, 1, 11)
        },
        new PersonDto
        {
            Id = 1008L,
            Coefficient = 1.7,
            FirstName = "Lisa",
            LastName = "Wilson",
            DateOfBirth = new DateTime(1974, 8, 6)
        },
        new PersonDto
        {
            Id = 1009L,
            Coefficient = -1.0,
            FirstName = "Robert",
            LastName = "Martinez",
            DateOfBirth = new DateTime(1980, 3, 17)
        },
        new PersonDto
        {
            Id = 1010L,
            Coefficient = -1.1,
            FirstName = "Jessica",
            LastName = "Anderson",
            DateOfBirth = new DateTime(1999, 12, 30)
        }
    };

    protected override SymmetricLens<TabularData, IEnumerable<PersonDto>> _lens => SymmetricTabularDataDtoLenses.Construct<PersonDto>();
}
