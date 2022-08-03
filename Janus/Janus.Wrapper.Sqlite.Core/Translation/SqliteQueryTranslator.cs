using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Commons.QueryModels;
using Janus.Wrapper.Core.Translation;
using Janus.Wrapper.Sqlite.Core.LocalQuerying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Sqlite.Core.Translation;
internal class SqliteQueryTranslator : ILocalQueryTranslator<SqliteQuery, string, string, string>
{
    public Result<SqliteQuery> Translate(Query query)
        => TranslateSelection(query.Selection)
            .Bind(selection => TranslateJoining(query.Joining).Map(joining => (selection, joining)))
            .Bind(result => TranslateProjection(query.Projection).Map(projection => (result.selection, result.joining, projection)))
            .Map(((string selection, string joining, string projection) result) 
                => new SqliteQuery(query.OnTableauId, result.selection, result.joining, result.projection));

    public Result<string> TranslateJoining(Option<Joining> joining)
        => ResultExtensions.AsResult(
            () => joining
                    ? "" // TODO
                    : "");

    public Result<string> TranslateProjection(Option<Projection> projection)
        => ResultExtensions.AsResult(
            () => projection 
                    ? $"SELECT {string.Join(",", projection.Value.IncludedAttributeIds)}"
                    : "SELECT *");

    public Result<string> TranslateSelection(Option<Selection> selection)
    {
        throw new NotImplementedException();
    }
}
