using Janus.Utils.Logging;
using Janus.Wrapper.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.ConsoleApp.Displays;
public class CurrentSchemaDisplay : BaseDisplay
{
    private readonly ILogger<CurrentSchemaDisplay>? _logger;
    public CurrentSchemaDisplay(WrapperController wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<CurrentSchemaDisplay>();
    }

    public override string Title => "CURRENT SCHEMA";

    protected async override Task<Result> Display()
        => (await _wrapperController.GetSchema())
            .Pass(r => Console.WriteLine(r ? r.Data : r.Message))
            .Match(data => Result.OnSuccess(), Result.OnFailure);
}
