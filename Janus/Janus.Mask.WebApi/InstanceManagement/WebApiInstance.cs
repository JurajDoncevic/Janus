using Janus.Base;
using Janus.Base.Resulting;
using Janus.Commons.SchemaModels;
using Janus.Mask.WebApi;
using Janus.Mask.WebApi.InstanceManagement;
using Janus.Mask.WebApi.InstanceManagement.Providers;
using Janus.Mask.WebApi.InstanceManagement.Templates;
using Janus.Mask.WebApi.InstanceManagement.Typing;
using Janus.Mask.WebApi.MaskedSchemaModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json;

namespace JanusGenericMask.InstanceManagement.Web;
internal class WebApiInstance
{
    private WebApplication? _webApp;
    private Task? _runningApplication;
    private readonly WebApiOptions _webApiOptions;
    private readonly WebApiMaskCommandManager _commandManager;
    private readonly WebApiMaskQueryManager _queryManager;
    private readonly WebApiMaskSchemaManager _schemaManager;
    private IHostApplicationLifetime? _applicationLifetime;
    private TypeFactory? _typeFactory;
    private readonly Janus.Logging.ILogger<WebApiInstance>? _localLogger;
    private readonly Janus.Logging.ILogger? _logger;
    private Option<DataSource> _dataSourceSchema;

    public WebApiInstance(WebApiOptions webApiOptions, WebApiMaskCommandManager commandManager, WebApiMaskQueryManager queryManager, WebApiMaskSchemaManager schemaManager, Janus.Logging.ILogger? logger = null)
    {
        _webApiOptions = webApiOptions;
        _commandManager = commandManager;
        _queryManager = queryManager;
        _schemaManager = schemaManager;
        _dataSourceSchema = Option<DataSource>.None;
        _localLogger = logger?.ResolveLogger<WebApiInstance>();
        _logger = logger;
    }

    public Result StartApplication(Option<DataSource> dataSourceSchema)
        => Results.AsResult(() =>
        {
            _dataSourceSchema = dataSourceSchema ? dataSourceSchema : _dataSourceSchema;
            if (!_dataSourceSchema)
            {
                return Results.OnFailure("Can't start Web API instance without a provided schema.");
            }
            var dataSource = _dataSourceSchema.Value;

            var builder = WebApplication.CreateBuilder();
            var urls = new string[] { $"http://127.0.0.1:{_webApiOptions.ListenPort}" };
            if (_webApiOptions.UseSSL)
            {
                urls = urls.Append($"https://127.0.0.1:{_webApiOptions.ListenPortSecure ?? (_webApiOptions.ListenPort + 1)}").ToArray();
            }

            builder.WebHost.UseUrls(urls);


            builder.Services.AddSingleton<WebApiOptions>(_webApiOptions);

            builder.Logging.ClearProviders();

            if (_logger is not null)
            {
                builder.Services.AddSingleton<Janus.Logging.ILogger>(provider => _logger);
            }

            var webApiTyping =
                _schemaManager.CurrentMaskedSchema
                              .Match(
                                schema => schema,
                                () => throw new Exception("No masked schema could be generated.")
                                );
            _typeFactory = new TypeFactory();

            var genericControllerProvider = new GenericControllerFeatureProvider(webApiTyping.ControllerTypings, _typeFactory);
            // Add services to the container.
            builder.Services.AddControllers()
                            .ConfigureApplicationPartManager(apm => apm.FeatureProviders.Add(genericControllerProvider));


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc($"{dataSource.Version}", new OpenApiInfo { Title = $"{dataSource.Name} API", Version = $"{dataSource.Version}" });
                c.DocumentFilter<BaseDtoDocumentFilter>();
            });

            builder.Services.AddSingleton(_ => new ProviderFactory(_commandManager, _queryManager));

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseSwagger();
            app.UseSwaggerUI(conf =>
            {
                conf.SwaggerEndpoint($"/swagger/{dataSource.Version}/swagger.json", $"{dataSource.Version}");
                conf.RoutePrefix = string.Empty;
                conf.DisplayRequestDuration();
            });

            //app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            _webApp = app;

            _runningApplication = app.RunAsync();


            _applicationLifetime = app.Services.GetService<IHostApplicationLifetime>();

            return Results.OnSuccess("Web API started");
        });

    public Result StopApplication()
        => Results.AsResult(() =>
        {
            _applicationLifetime?.StopApplication();
            _runningApplication?.Wait();
            _typeFactory?.Dispose();
            _runningApplication?.Dispose();

            return Results.OnSuccess("Web API stoppped");
        });

    public bool IsRunning()
        => _runningApplication is not null &&
           _runningApplication.Status == TaskStatus.WaitingForActivation;
}


public class BaseDtoDocumentFilter : IDocumentFilter
{
    private readonly IEnumerable<Type> _dtos;
    private readonly IEnumerable<ControllerTyping> _controllerTypings;

    public BaseDtoDocumentFilter(IEnumerable<ControllerTyping> controllerTypings)
    {
        _dtos = DtoTypeHelpers.GetDtoTypesFromDynamicAssembly();
        _controllerTypings = controllerTypings;
    }
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {

        OpenApiMediaType GetOpenApiMediaType(Type type)
        {
            var activedInstance = Activator.CreateInstance(type);
            var exampleInstance = JsonSerializer.Serialize(activedInstance, type, new JsonSerializerOptions { WriteIndented = true });
            // Get media type with example for the type  
            return new OpenApiMediaType()
            {
                Example = new OpenApiString(exampleInstance),
                Schema = new OpenApiSchema
                {
                    Type = type.Name
                }
            };
        }

        foreach (var dto in _dtos)
        {
            // Define sample payload  
            swaggerDoc.Components.RequestBodies.Add(dto.Name, new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>()
                        {
                            { "application/json-patch+json", GetOpenApiMediaType(dto)},
                            { "application/json", GetOpenApiMediaType(dto)},
                            { "text/json", GetOpenApiMediaType(dto)},
                            { "application/*+json", GetOpenApiMediaType(dto)}
                        }
            });
        }

    }

}

internal static class DtoTypeHelpers
{
    internal static IEnumerable<Type> GetDtoTypes(this Assembly assembly)
    {
        IEnumerable<Type> _dtoTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && t.IsPublic && t.IsSubclassOf(typeof(BaseDto)))
            .OrderBy(t => t.Name);

        return _dtoTypes;
    }

    internal static IEnumerable<Type> GetDtoTypesFromDynamicAssembly()
    {
        var asms = AppDomain.CurrentDomain.GetAssemblies();
        return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.IsDynamic && a.GetName().Name.Equals("DynamicAssembly"))?
                .GetDtoTypes() ?? Enumerable.Empty<Type>();
    }
}