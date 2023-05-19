using Janus.Commons.DataModels;
using Janus.Lenses.Implementations;
using Janus.Lenses.Tests.Implementations.TestingClasses;

namespace Janus.Lenses.Tests.Implementations;
public sealed class RowDataDtoLensTests : SymmetricLensTestingFramework<RowData, PersonDto>
{
    protected override RowData _x =>
        RowData.FromDictionary(new Dictionary<string, object?>
        {
                { "Id", 1L },
                { "FirstName", "John" },
                { "LastName", "Doe" },
                { "DateOfBirth", new DateTime(1965, 1, 1)}
        });
    protected override PersonDto _y => new PersonDto
    {
        Id = 1L,
        FirstName = "John",
        LastName = "Doe",
        DateOfBirth = new DateTime(1965, 1, 1)
    };

    protected override SymmetricLens<RowData, PersonDto> _lens => SymmetricRowDataDtoLenses.Construct<PersonDto>();
}
