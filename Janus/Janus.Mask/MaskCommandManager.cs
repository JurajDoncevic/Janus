using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Communication.Remotes;
using Janus.Components;

namespace Janus.Mask;
public sealed class MaskCommandManager : IDelegatingCommandManager
{
    public Task<Result> RunCommandOnComponent(BaseCommand command, RemotePoint remotePoint)
        => Results.AsResult(async () =>
        {
            return Results.OnException(new NotImplementedException());
        });
}
