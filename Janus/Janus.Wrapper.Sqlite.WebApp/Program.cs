using Janus.Commons;
using Janus.Communication;
using Janus.Communication.Nodes.Implementations;
using Janus.Components.Persistence;
using Janus.Serialization;
using Janus.Serialization.Avro;
using Janus.Serialization.Bson;
using Janus.Serialization.Json;
using Janus.Serialization.MongoBson;
using Janus.Serialization.Protobufs;
using Janus.Wrapper;
using Janus.Wrapper.Persistence;
using Janus.Wrapper.Persistence.LiteDB;
using Janus.Wrapper.SchemaInference;
using Janus.Wrapper.Sqlite;
using Janus.Wrapper.Sqlite.LocalCommanding;
using Janus.Wrapper.Sqlite.LocalQuerying;
using Janus.Wrapper.Sqlite.SchemaInference;
using Janus.Wrapper.Sqlite.Translation;
using Janus.Wrapper.Sqlite.WebApp;
using LiteDB;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using ProtoBuf.Meta;

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

// load wrapper configuration
var wrapperConfiguration =
    configuration.GetSection("WrapperConfiguration")
                 .Get<WrapperConfiguration>();
// load web configuration
var webConfiguration =
    configuration.GetSection("WebConfiguration")
                 .Get<WebConfiguration>();

// set port for web host
builder.WebHost.UseUrls($"http://127.0.0.1:{webConfiguration.Port}", $"https://127.0.0.1:{webConfiguration.Port + 1}");

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
    wrapperConfiguration.CommunicationFormat,
    wrapperConfiguration.NetworkAdapterType))
    throw new Exception($"Incompatible communication data format {wrapperConfiguration.CommunicationFormat} for network adapter type {wrapperConfiguration.NetworkAdapterType}");

// set the serialization provider and network adapter
var _ =
wrapperConfiguration.NetworkAdapterType switch
{
    NetworkAdapterTypes.TCP =>
        builder.Services
        .AddSingleton<IBytesSerializationProvider>(
            wrapperConfiguration.CommunicationFormat switch
            {
                CommunicationFormats.AVRO => new AvroSerializationProvider(),
                CommunicationFormats.BSON => new BsonSerializationProvider(),
                CommunicationFormats.MONGO_BSON => new MongoBsonSerializationProvider(),
                CommunicationFormats.PROTOBUFS => new ProtobufsSerializationProvider(),
                _ => new AvroSerializationProvider()
            })
        .AddSingleton<WrapperCommunicationNode>(serviceProvider =>
            CommunicationNodes.CreateTcpWrapperCommunicationNode(
                new Janus.Communication.Nodes.CommunicationNodeOptions(
                    wrapperConfiguration.NodeId,
                    wrapperConfiguration.ListenPort,
                    wrapperConfiguration.TimeoutMs
                    ),
                serviceProvider.GetService<IBytesSerializationProvider>()!,
                serviceProvider.GetService<Janus.Logging.ILogger>())),
    _ => throw new Exception("Unknown network adapter type")
};

// setup querying
builder.Services.AddSingleton<SqliteQueryTranslator>();
builder.Services.AddSingleton<SqliteDataTranslator>(provider => new SqliteDataTranslator(wrapperConfiguration.DataSourceName ?? wrapperConfiguration.NodeId));
builder.Services.AddSingleton<SqliteQueryExecutor>(provider => new SqliteQueryExecutor(wrapperConfiguration.SourceConnectionString));
builder.Services.AddSingleton<SqliteWrapperQueryManager>();

// setup commanding
builder.Services.AddSingleton<SqliteCommandTranslator>();
builder.Services.AddSingleton<SqliteCommandExecutor>(provider => new SqliteCommandExecutor(wrapperConfiguration.SourceConnectionString));
builder.Services.AddSingleton<SqliteWrapperCommandManager>();

// setup schema managment
builder.Services.AddSingleton<SqliteSchemaModelProvider>(provider => new SqliteSchemaModelProvider(wrapperConfiguration.SourceConnectionString));
builder.Services.AddSingleton<SchemaInferrer>(
    provider => new SchemaInferrer(provider.GetService<SqliteSchemaModelProvider>()!, wrapperConfiguration.DataSourceName ?? wrapperConfiguration.NodeId));
builder.Services.AddSingleton<SqliteWrapperSchemaManager>();

// setup controller
builder.Services.AddSingleton<SqliteWrapperManager>();

builder.Services.AddSingleton<ILiteDatabase, LiteDatabase>(services => new LiteDatabase(wrapperConfiguration.PersistenceConnectionString));
builder.Services.AddSingleton<JsonSerializationProvider>();
builder.Services.AddSingleton<IDataSourceInfoPersistence, DataSourceInfoPersistence>();
builder.Services.AddSingleton<IRemotePointPersistence, RemotePointPersistence>();
builder.Services.AddSingleton<WrapperPersistenceProvider>();

builder.Services.AddSingleton<SqliteWrapperOptions>(wrapperConfiguration.ToWrapperOptions());

builder.Services.AddSingleton<SqliteWrapperManager>();


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
app.Services.GetRequiredService<SqliteWrapperManager>();

await runTask;
