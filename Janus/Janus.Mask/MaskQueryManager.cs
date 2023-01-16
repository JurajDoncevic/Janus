using FunctionalExtensions.Base.Resulting;
using Janus.Commons.DataModels;
using Janus.Commons.QueryModels;
using Janus.Communication.Remotes;
using Janus.Components;

namespace Janus.Mask;
public class MaskQueryManager : IDelegatingQueryManager
{
    public Task<Result<TabularData>> RunQueryOn(Query query, RemotePoint remotePoint)
        => Results.AsResult<TabularData>(async () =>
        {
            return Results.OnException<TabularData>(new NotImplementedException());
        });
}
