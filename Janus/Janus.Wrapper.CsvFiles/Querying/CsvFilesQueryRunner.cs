using Janus.Commons.DataModels;
using Janus.Wrapper.Core.Querying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.CsvFiles.Querying;
public class CsvFilesQueryRunner : IWrapperQueryRunner<Query>
{
    private readonly string _dataSourceDirectoryPath;

    public CsvFilesQueryRunner(string dataSourceDirectoryPath)
    {
        _dataSourceDirectoryPath = dataSourceDirectoryPath;
    }

    public string DataSourceDirectoryPath => _dataSourceDirectoryPath;

    public async Task<Result<TabularData>> RunQuery(Query query)
        => await ResultExtensions.AsResult<TabularData>(async () =>
        {
            List<Dictionary<string, object>> values;
            Dictionary<string, Commons.SchemaModels.DataTypes> attributeDataTypes;
            List<string> columnPaths;
            if (query.Joining.Joins.Count == 0) // no joins in this query, work over one file
            {
                columnPaths =
                    File.ReadLines(AdaptPathToDataSourceLocation(query.OnFilePath) + ".csv")
                        .First()
                        .Split(";")
                        .Select(colName => query.OnFilePath + "/" + colName)
                        .ToList();

                values =
                    (await File.ReadAllLinesAsync(AdaptPathToDataSourceLocation(query.OnFilePath) + ".csv"))
                        .Skip(1)
                        .Map(line => line.Trim().Split(";").Map(Utils.InferAttributeType))
                        .Map(values => values.Mapi((idx, v) => (attrPath: columnPaths[(int)idx], value: v)))
                        .Map(attrValues => attrValues.ToDictionary(_ => _.attrPath, _ => _.value))
                        .ToList();

                attributeDataTypes =
                    (await File.ReadAllLinesAsync(AdaptPathToDataSourceLocation(query.OnFilePath) + ".csv"))
                        .Skip(1)
                        .Take(1)
                        .Map(line => line.Split(";").Mapi((idx, value) => (idx, dataType: Utils.InferAttributeDataType(value))))
                        .First()
                        .ToDictionary(_ => columnPaths[(int)_.idx], _ => _.dataType);
            }
            else
            {
                throw new Exception("Joining on this wrapper not yet supported");
            }

            var selectedValues =
                values.Where(query.Selection.Expression).ToList();

            var projectedValues =
                selectedValues.Select(values => query.Projection.ColumnPaths.ToDictionary(projAttrPath => projAttrPath, projAttrPath => values[projAttrPath]))
                              .ToList();

            var projectedDataTypes =
                attributeDataTypes.Where(attrDataType => query.Projection.ColumnPaths.Contains(attrDataType.Key))
                .ToDictionary(_ => _.Key, _ => _.Value);

            var resultBuilder = TabularDataBuilder.InitTabularData(projectedDataTypes);

            foreach (var row in projectedValues)
            {
                resultBuilder.AddRow(conf => conf.WithRowData(row!));
            }

            return resultBuilder.Build();
        });

    private string AdaptPathToDataSourceLocation(string fullPath)
        => Path.Join(_dataSourceDirectoryPath, fullPath.Split("/", 2).ElementAt(1));
}
