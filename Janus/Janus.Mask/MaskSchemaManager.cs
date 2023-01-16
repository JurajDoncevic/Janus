using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;
using Janus.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Mask;
public class MaskSchemaManager : IComponentSchemaManager, IDelegatingSchemaManager
{
    public Option<DataSource> GetCurrentOutputSchema()
        => Option<DataSource>.None;

    public Task<Result<DataSource>> GetSchemaFrom(RemotePoint remotePoint)
        => Results.AsResult(async () =>
        {
            return Results.OnException<DataSource>(new NotImplementedException());
        });

    public Task<Dictionary<RemotePoint, Result<DataSource>>> GetSchemasFromComponents()
    {
        throw new NotImplementedException();
    }

    public Task<Result<DataSource>> LoadSchema(RemotePoint remotePoint)
        => Results.AsResult(async () =>
        {
            return Results.OnException<DataSource>(new NotImplementedException());
        });

    public Task<Result<DataSource>> ReloadOutputSchema()
        => Results.AsResult(async () =>
        {
            return Results.OnException<DataSource>(new NotImplementedException());
        });

    public Task<IEnumerable<Result<DataSource>>> ReloadSchemas()
    {
        throw new NotImplementedException();
    }

    public Result UnloadSchema(RemotePoint remotePoint)
    {
        throw new NotImplementedException();
    }
}
