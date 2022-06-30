using Janus.Commons.SchemaModels;
using Janus.Utils.Logging;
using Janus.Wrapper.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.ConsoleApp.Displays;
public class ReloadSchemaDisplay : BaseDisplay
{
    private readonly ILogger<ReloadSchemaDisplay>? _logger;
    public ReloadSchemaDisplay(CsvFilesWrapperController wrapperController, ILogger? logger = null) : base(wrapperController)
    {
        _logger = logger?.ResolveLogger<ReloadSchemaDisplay>();
    }

    public override string Title => "RELOADING SCHEMA";

    protected async override Task<Result> Display()
        => (await _wrapperController.ReloadSchema())
            .Pass(r => Console.WriteLine("Reloaded:\n" + (r ? r.Data!.ToString() : r.Message)))
            .Match(data => Result.OnSuccess(), Result.OnFailure);
}
