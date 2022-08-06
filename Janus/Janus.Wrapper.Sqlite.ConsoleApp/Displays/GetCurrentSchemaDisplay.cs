using FunctionalExtensions.Base;
using FunctionalExtensions.Base.Results;
using Janus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Sqlite.ConsoleApp.Displays;
public class GetCurrentSchemaDisplay : BaseDisplay
{
    private readonly ILogger<GetCurrentSchemaDisplay>? _logger;

    public GetCurrentSchemaDisplay(SqliteWrapperController wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<GetCurrentSchemaDisplay>();
    }

    public override string Title => "CURRENT SCHEMA";

    protected override async Task<Result> Display()
        => (await _wrapperController.GetSchema())
            .Pass(result => Console.WriteLine(result.Data?.ToString()),
                  result => Console.WriteLine(result.Message))
            .Bind(r => Result.OnSuccess());


}
