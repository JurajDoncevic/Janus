using Janus.Commons;
using Janus.Communication;
using Janus.Communication.Nodes.Implementations;
using Janus.Components.Persistence;
using Janus.Mask.Persistence;
using Janus.Mask.Persistence.LiteDB;
using Janus.Mask.Sqlite;
using Janus.Mask.Sqlite.Translation;
using Janus.Mask.Sqlite.WebApp;
using Janus.Serialization;
using Janus.Serialization.Avro;
using Janus.Serialization.Bson;
using Janus.Serialization.Json;
using Janus.Serialization.MongoBson;
using Janus.Serialization.Protobufs;
using LiteDB;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// take the appsettings file depending on the environment
IConfiguration configuration;
if (builder.Environment.IsDevelopment())
{
    configuration = builder.Configuration.AddJsonFile("appsettings.Development.json").Build();
}
else
{
    configuration = builder.Configuration.AddJsonFile("appsettings.json").Build();
}

// load mediator configuration
var maskConfiguration =
    configuration.GetSection("MaskConfiguration")
                 .Get<MaskConfiguration>();
// load web configuration
var webConfiguration =
    configuration.GetSection("WebConfiguration")
                 .Get<WebConfiguration>();

// set port for web host
builder.WebHost.UseUrls($"{webConfiguration.AllowedHttpHost}:{webConfiguration.Port}", $"{webConfiguration.AllowedHttpsHost}:{webConfiguration.Port + 1}");

// Add services to the container.
builder.Services.AddControllersWithViews();

// load services
builder.Host.ConfigureLogging((hostContext, loggingBuilder) => // setup logging
{
    var loggingSection = hostContext.Configuration.GetSection("NLog");
    if (loggingSection != null)
    {
        LogManager.Configuration = new NLogLoggingConfiguration(loggingSection);
    }

}).UseNLog();

builder.Services.AddSingleton(configuration); // register the IConfig
builder.Services.AddSingleton<Janus.Logging.ILogger, Janus.Logging.Logger>(); // register the concrete logger

// check if the given data format and network adapter type are compatible
if (!Utils.IsDataFormatCompatibleWithAdapter(
    maskConfiguration.CommunicationFormat,
    maskConfiguration.NetworkAdapterType))
    throw new Exception($"Incompatible communication data format {maskConfiguration.CommunicationFormat} for network adapter type {maskConfiguration.NetworkAdapterType}");

// set the serialization provider and network adapter
var _ =
maskConfiguration.NetworkAdapterType switch
{
    NetworkAdapterTypes.TCP =>
        builder.Services
        .AddSingleton<IBytesSerializationProvider>(
            maskConfiguration.CommunicationFormat switch
            {
                CommunicationFormats.AVRO => new AvroSerializationProvider(),
                CommunicationFormats.BSON => new BsonSerializationProvider(),
                CommunicationFormats.MONGO_BSON => new MongoBsonSerializationProvider(),
                CommunicationFormats.PROTOBUFS => new ProtobufsSerializationProvider(),
                _ => new AvroSerializationProvider()
            })
        .AddSingleton<MaskCommunicationNode>(serviceProvider =>
            CommunicationNodes.CreateTcpMaskCommunicationNode(
                new Janus.Communication.Nodes.CommunicationNodeOptions(
                    maskConfiguration.NodeId,
                    maskConfiguration.ListenPort,
                    maskConfiguration.TimeoutMs
                    ),
                serviceProvider.GetService<IBytesSerializationProvider>()!,
                serviceProvider.GetService<Janus.Logging.ILogger>())),
    _ => throw new Exception("Unknown network adapter type")
};


builder.Services.AddSingleton<SqliteQueryTranslator>();
builder.Services.AddSingleton<SqliteCommandTranslator>();
builder.Services.AddSingleton<SqliteSchemaTranslator>();

builder.Services.AddSingleton<SqliteMaskQueryManager>();
builder.Services.AddSingleton<SqliteMaskCommandManager>();
builder.Services.AddSingleton<SqliteMaskSchemaManager>();

builder.Services.AddSingleton<JsonSerializationProvider>();
builder.Services.AddSingleton<ILiteDatabase, LiteDatabase>(services => new LiteDatabase(maskConfiguration.PersistenceConnectionString));
builder.Services.AddSingleton<IDataSourceInfoPersistence, DataSourceInfoPersistence>();
builder.Services.AddSingleton<IRemotePointPersistence, RemotePointPersistence>();
builder.Services.AddSingleton<MaskPersistenceProvider>();

builder.Services.AddSingleton<SqliteMaskOptions>(maskConfiguration.ToMaskOptions());

builder.Services.AddSingleton<SqliteMaskManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    //app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

var runTask = app.RunAsync();

// preload mediator manager, so the web interface doesn't need to be accessed for the app to work
var manager = app.Services.GetService<SqliteMaskManager>();

await runTask;