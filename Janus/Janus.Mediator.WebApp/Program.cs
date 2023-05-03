using Janus.Commons;
using Janus.Communication.Nodes.Implementations;
using Janus.Communication;
using Janus.Mediator;
using Janus.Mediator.WebApp;
using Janus.Serialization;
using NLog;
using NLog.Extensions.Hosting;
using NLog.Extensions.Logging;
using Janus.Serialization.Avro;
using Janus.Serialization.MongoBson;
using Janus.Serialization.Bson;
using Janus.Serialization.Protobufs;
using Janus.Mediator.Persistence;
using Janus.Mediator.Persistence.LiteDB;
using Janus.Components.Persistence;
using Janus.Serialization.Json;
using LiteDB;

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
var mediatorConfiguration = 
    configuration.GetSection("MediatorConfiguration")
                 .Get<MediatorConfiguration>();
// load web configuration
var webConfiguration =
    configuration.GetSection("WebConfiguration")
                 .Get<WebConfiguration>();

// set port for web host
builder.WebHost.UseUrls($"http://127.0.0.1:{webConfiguration.Port}", $"https://127.0.0.1:{webConfiguration.Port+1}");

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
    mediatorConfiguration.CommunicationFormat,
    mediatorConfiguration.NetworkAdapterType))
    throw new Exception($"Incompatible communication data format {mediatorConfiguration.CommunicationFormat} for network adapter type {mediatorConfiguration.NetworkAdapterType}");

// set the serialization provider and network adapter
var _ =
mediatorConfiguration.NetworkAdapterType switch
{
    NetworkAdapterTypes.TCP =>
        builder.Services
        .AddSingleton<IBytesSerializationProvider>(
            mediatorConfiguration.CommunicationFormat switch
            {
                CommunicationFormats.AVRO => new AvroSerializationProvider(),
                CommunicationFormats.BSON => new BsonSerializationProvider(),
                CommunicationFormats.MONGO_BSON => new MongoBsonSerializationProvider(),
                CommunicationFormats.PROTOBUFS => new ProtobufsSerializationProvider(),
                _ => new AvroSerializationProvider()
            })
        .AddSingleton<MediatorCommunicationNode>(serviceProvider =>
            CommunicationNodes.CreateTcpMediatorCommunicationNode(
                new Janus.Communication.Nodes.CommunicationNodeOptions(
                    mediatorConfiguration.NodeId,
                    mediatorConfiguration.ListenPort,
                    mediatorConfiguration.TimeoutMs
                    ),
                serviceProvider.GetService<IBytesSerializationProvider>()!,
                serviceProvider.GetService<Janus.Logging.ILogger>())),
    _ => throw new Exception("Unknown network adapter type")
};

builder.Services.AddSingleton<MediatorQueryManager>();
builder.Services.AddSingleton<MediatorCommandManager>();
builder.Services.AddSingleton<MediatorSchemaManager>();

builder.Services.AddSingleton<ILiteDatabase, LiteDatabase>(services => new LiteDatabase(mediatorConfiguration.PersistenceConnectionString));
builder.Services.AddSingleton<JsonSerializationProvider>();
builder.Services.AddSingleton<IDataSourceInfoPersistence, DataSourceInfoPersistence>();
builder.Services.AddSingleton<IRemotePointPersistence, RemotePointPersistence>();
builder.Services.AddSingleton<MediatorPersistenceProvider>();

builder.Services.AddSingleton<MediatorOptions>(mediatorConfiguration.ToMediatorOptions());

builder.Services.AddSingleton<MediatorManager>();


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
var mediatorManager = app.Services.GetRequiredService<MediatorManager>();

await runTask;