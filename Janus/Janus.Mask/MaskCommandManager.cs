using FunctionalExtensions.Base.Resulting;
using Janus.Commons.CommandModels;
using Janus.Communication.Remotes;
using Janus.Components;

namespace Janus.Mask;
public sealed class MaskCommandManager : IDelegatingCommandManager
{
    public async Task<Result> RunCommand(BaseCommand command)
        => await Results.AsResult(async () =>
        {
            return Results.OnException(new NotImplementedException());
        });

    public async Task<Result> RunCommandOnComponent(BaseCommand command, RemotePoint remotePoint)
        => await Results.AsResult(async () =>
        {
            return Results.OnException(new NotImplementedException());
        });
}
