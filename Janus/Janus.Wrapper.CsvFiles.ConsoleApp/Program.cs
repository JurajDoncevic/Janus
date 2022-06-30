using CommandLine;
using Janus.Communication;
using Janus.Communication.NetworkAdapters;
using Janus.Communication.NetworkAdapters.Tcp;
using Janus.Communication.Nodes;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Remotes;
using Janus.Wrapper.CsvFiles.ConsoleApp;
using Janus.Wrapper.CsvFiles.ConsoleApp.Options;
using Janus.Wrapper.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using Janus.Wrapper.Core.SchemaInferrence;
using Janus.Wrapper.CsvFiles.SchemaInferrence;
using Janus.Wrapper.CsvFiles;
using Janus.Wrapper.CsvFiles.Querying;
using Janus.Wrapper.Core.Querying;
using Janus.Components.Translation;
using Janus.Wrapper.CsvFiles.Translation;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configurationBuilder) =>
    {
        Parser.Default.ParseArguments<CommandLineOptions>(args)
            .WithParsed(options =>
            {
                if (options.ConfigurationFilePath != null) // if the configuration file is given
                {
                    configurationBuilder.AddJsonFile(options.ConfigurationFilePath);
                }
                else // the configuration is given via CL arguments
                {
                    configurationBuilder.AddInMemoryCollection(
                        new Dictionary<string, string>
                        {
                            { "ComponentOptions:NodeId", options.NodeId },
                            { "ComponentOptions:ListenPort", options.ListenPort.ToString() },
                            { "ComponentOptions:TimeoutMs", options.TimeoutMs.ToString() },
                            { "ComponentOptions:DataSourcePath", options.DataSourcePath.ToString() },
                            { "ComponentOptions:StartupRemotePoints", "[]" },
                        });
                }

                configurationBuilder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        { "ApplicationOptions:StartWithCLI", options.StartWithCLI.ToString() }
                    });
            })
            .WithNotParsed(errors =>
            {
            });
    })
    .ConfigureLogging((hostContext, loggingBuilder) =>
    {
        var loggingSection = hostContext.Configuration.GetSection("NLog");
        if (loggingSection != null)
        {
            LogManager.Configuration = new NLogLoggingConfiguration(loggingSection);
        }

    }).UseNLog()
    .ConfigureServices((hostContext, services) =>
    {
        var wrapperOptions = hostContext.Configuration
                                .GetSection("ComponentOptions")
                                .Get<WrapperConfigurationOptions>()
                                .ToWrapperOptions();

        var applicationOptions = hostContext.Configuration
                                    .GetSection("ApplicationOptions")
                                    .Get<ApplicationConfigurationOptions>()
                                    .ToApplicationOptions();

        services.AddSingleton<IConfiguration>(hostContext.Configuration);
        services.AddSingleton<Janus.Utils.Logging.ILogger, Janus.Utils.Logging.Logger>();
        services.AddSingleton<WrapperCommunicationNode>(serviceProvider =>
            CommunicationNodes.CreateTcpWrapperCommunicationNode(
                new Janus.Communication.Nodes.CommunicationNodeOptions(
                    wrapperOptions.NodeId,
                    wrapperOptions.ListenPort,
                    wrapperOptions.TimeoutMs
                    ),
                serviceProvider.GetService<Janus.Utils.Logging.ILogger>()
                )
            );

        services.AddSingleton<ISchemaModelProvider, CsvFilesProvider>(serviceProvider => new CsvFilesProvider(wrapperOptions.DataSourcePath, ';'));
        services.AddSingleton<SchemaInferrer>();
        services.AddSingleton<IWrapperQueryRunner<Query>, CsvFilesQueryRunner>(serviceProvider => new CsvFilesQueryRunner(wrapperOptions.DataSourcePath));
        services.AddSingleton<IQueryTranslator<Janus.Commons.QueryModels.Query, Janus.Commons.QueryModels.Selection, Janus.Commons.QueryModels.Joining, Janus.Commons.QueryModels.Projection, Query, Selection, Joining, Projection>, CsvFilesQueryTranslator>();
        services.AddSingleton<WrapperQueryManager<Query, Selection, Joining, Projection>>();
        services.AddSingleton<WrapperCommandManager>();
        services.AddSingleton<WrapperSchemaManager>();
        services.AddSingleton<CsvFilesWrapperController>();
        services.AddSingleton<WrapperOptions>(wrapperOptions);
        services.AddSingleton<ApplicationOptions>(applicationOptions);
        services.AddSingleton<Application>();

    })
    .Build();
await host.StartAsync();
host.Services.GetService<Application>();
host.WaitForShutdown();
