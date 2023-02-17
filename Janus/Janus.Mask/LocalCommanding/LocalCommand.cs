using Janus.Commons.SchemaModels;

namespace Janus.Mask.LocalCommanding;
public abstract class LocalCommand
{
    private readonly TableauId _target;

    protected LocalCommand(TableauId target)
    {
        _target = target;
    }

    public TableauId Target => _target;
}
