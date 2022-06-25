using Janus.Wrapper.Core.SchemaInferrence;
using Janus.Wrapper.Core.SchemaInferrence.Model;
using System.IO;

namespace Janus.Wrapper.CsvFiles;
public class CsvFileSystemSchemaModelProvider : ISchemaModelProvider
{
    private readonly string _rootDirectoryPath;

    public CsvFileSystemSchemaModelProvider(string rootDirectoryPath)
    {
        _rootDirectoryPath = Path.GetFullPath(rootDirectoryPath);
    }

    public Result AttributeExists(string schemaName, string tableauName, string attributeName)
        => ResultExtensions.AsResult(
            () => File.ReadLines(Path.Combine(_rootDirectoryPath, schemaName, tableauName)).First()
                      .Identity()
                      .Map(headerLine => headerLine.Trim().Split(";"))
                      .Data
                      .Contains(attributeName));

    public Result<IEnumerable<AttributeInfo>> GetAttributes(string schemaName, string tableauName)
        => ResultExtensions.AsResult(
            () => File.ReadLines(Path.Combine(_rootDirectoryPath, schemaName, tableauName)).First()
                      .Identity()
                      .Map(headerLine => headerLine.Trim().Split(";"))
                      .Data
                      .Mapi((idx, attributeHeader) => new AttributeInfo(attributeHeader, Commons.SchemaModels.DataTypes.STRING, false, true, (int)idx)));

    public Result<DataSourceInfo> GetDataSource()
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type. Never occurs - AsResult handles null as failure
        => ResultExtensions.AsResult(() => Path.GetDirectoryName(_rootDirectoryPath)
                                               .Identity()
                                               .Map(dirName => dirName != null ? new DataSourceInfo(dirName) : null)
                                               .Data);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

    public Result<IEnumerable<SchemaInfo>> GetSchemas()
        => ResultExtensions.AsResult(
            () => Directory.GetDirectories(_rootDirectoryPath)
                           .Map(dirName => new SchemaInfo(dirName)));

    public Result<IEnumerable<TableauInfo>> GetTableaus(string schemaName)
        => ResultExtensions.AsResult(
            () => Directory.GetDirectories(Path.Combine(_rootDirectoryPath, schemaName))
                           .Map(fileName => new TableauInfo(fileName)));

    public Result SchemaExists(string schemaName)
        => ResultExtensions.AsResult(() => Directory.Exists(Path.Combine(_rootDirectoryPath, schemaName)));

    public Result TableauExists(string schemaName, string tableauName)
        => ResultExtensions.AsResult(() => File.Exists(Path.Combine(_rootDirectoryPath, schemaName, tableauName)));
}
