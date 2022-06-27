using Janus.Commons.SchemaModels;
using Janus.Wrapper.Core.SchemaInferrence;
using Janus.Wrapper.Core.SchemaInferrence.Model;
using System.IO;
using System.Text.RegularExpressions;

namespace Janus.Wrapper.CsvFiles;
public class CsvFilesProvider : ISchemaModelProvider
{
    private readonly char _delimiter;
    private readonly string _rootDirectoryPath;

    public CsvFilesProvider(string rootDirectoryPath, char delimiter)
    {
        _delimiter = delimiter;
        _rootDirectoryPath = Path.GetFullPath(rootDirectoryPath);
    }

    public Result AttributeExists(string schemaName, string tableauName, string attributeName)
        => ResultExtensions.AsResult(
            () => File.ReadLines(Path.Combine(_rootDirectoryPath, schemaName, tableauName)).First()
                      .Identity()
                      .Map(headerLine => headerLine.Trim().Split(_delimiter))
                      .Data
                      .Contains(attributeName));

    public Result<IEnumerable<AttributeInfo>> GetAttributes(string schemaName, string tableauName)
        => ResultExtensions.AsResult(
            () => File.ReadLines(Path.Combine(_rootDirectoryPath, schemaName, tableauName) + ".csv").First()
                      .Identity()
                      .Map(headerLine => headerLine.Trim().Split(_delimiter))
                      .Data
                      .Mapi((idx, attributeHeader) => new AttributeInfo(attributeHeader, Commons.SchemaModels.DataTypes.STRING, false, true, (int)idx)))
            .Bind(attributeInfos => ResultExtensions.AsResult(
                                        () => File.ReadLines(Path.Combine(_rootDirectoryPath, schemaName, tableauName) + ".csv").Skip(1).First()
                                                 .Identity()
                                                 .Map(dataLine => dataLine.Trim().Split(_delimiter))
                                                 .Data
                                                 .Map(InferAttributeType))
                                                 .Map(r => attributeInfos.Mapi((idx, a) => new AttributeInfo(a.Name, r.ElementAt((int)idx), a.IsPrimaryKey, a.IsNullable, a.Ordinal))));

    public Result<DataSourceInfo> GetDataSource()
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type. Never occurs - AsResult handles null as failure
        => ResultExtensions.AsResult(() => Path.GetFileName(_rootDirectoryPath)
                                               .Identity()
                                               .Map(dirPath => Path.GetFileName(dirPath)) 
                                               .Map(dirName => dirName != null ? new DataSourceInfo(dirName) : null)
                                               .Data);
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.

    public Result<IEnumerable<SchemaInfo>> GetSchemas()
        => ResultExtensions.AsResult(
            () => Directory.EnumerateDirectories(_rootDirectoryPath)
                           .Map(filePath => Path.GetFileName(filePath))
                           .Map(dirName => new SchemaInfo(dirName!)));

    public Result<IEnumerable<TableauInfo>> GetTableaus(string schemaName)
        => ResultExtensions.AsResult(
            () => Directory.EnumerateFiles(Path.Combine(_rootDirectoryPath, schemaName))
                           .Map(filePath => Path.GetFileNameWithoutExtension(filePath))
                           .Map(fileName => new TableauInfo(fileName)));

    public Result SchemaExists(string schemaName)
        => ResultExtensions.AsResult(() => Directory.Exists(Path.Combine(_rootDirectoryPath, schemaName)));

    public Result TableauExists(string schemaName, string tableauName)
        => ResultExtensions.AsResult(() => File.Exists(Path.Combine(_rootDirectoryPath, schemaName, tableauName) + ".csv"));

    private DataTypes InferAttributeType(string value)
    {
        if (Regex.IsMatch(value.Trim(), @"^0|-?[1-9][0-9]*$") && int.TryParse(value, out _))
            return DataTypes.INT;
        if (Regex.IsMatch(value.Trim(), @"^-?([1-9][0-9]*|0)[\.|,][0-9]+$") && double.TryParse(value, out _))
            return DataTypes.DECIMAL;
        if (bool.TryParse(value.Trim(), out _))
            return DataTypes.BOOLEAN;
        if (DateTime.TryParse(value.Trim(), out _))
            return DataTypes.DATETIME;
        return DataTypes.STRING;
    }

}
