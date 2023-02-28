using Janus.Commons.SchemaModels;

namespace Janus.Mask.MaskedCommandModel;
public abstract class MaskedCommand
{
    private readonly TableauId _target;

    protected MaskedCommand(TableauId target)
    {
        _target = target;
    }

    public TableauId Target => _target;
}
