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
            Dictionary<string, Commons.SchemaModels.DataTypes> attributeDataTypes;

            List<string> columnPaths =
                File.ReadLines(AdaptPathToDataSourceLocation(query.OnFilePath) + ".csv")
                    .First()
                    .Split(";")
                    .Select(colName => query.OnFilePath + "/" + colName)
                    .ToList();

            List<Dictionary<string, object>> values =
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

            if (query.Joining.Joins.Count > 0) // there are joins in the query
            {
                foreach (var join in query.Joining.Joins.OrderBy(_ => _.ForeignKeyFilePath.Equals(query.OnFilePath)))
                {

                    List<string> joiningColumnPaths =
                        File.ReadLines(AdaptPathToDataSourceLocation(join.PrimaryKeyFilePath) + ".csv")
                            .First()
                            .Split(";")
                            .Select(colName => join.PrimaryKeyFilePath + "/" + colName)
                            .ToList();

                    (await File.ReadAllLinesAsync(AdaptPathToDataSourceLocation(join.PrimaryKeyFilePath) + ".csv"))
                        .Skip(1)
                        .Take(1)
                        .Map(line => line.Split(";").Mapi((idx, value) => (idx, dataType: Utils.InferAttributeDataType(value))))
                        .First()
                        .ToDictionary(_ => joiningColumnPaths[(int)_.idx], _ => _.dataType)
                        .ToList()
                        .ForEach(x => attributeDataTypes.Add(x.Key, x.Value));

                    var primaryKeydata =
                        (await File.ReadAllLinesAsync(AdaptPathToDataSourceLocation(join.PrimaryKeyFilePath) + ".csv"))
                            .Skip(1)
                            .Map(line => line.Trim().Split(";").Map(Utils.InferAttributeType))
                            .Map(values => values.Mapi((idx, v) => (attrPath: joiningColumnPaths[(int)idx], value: v)))
                            .Map(attrValues => attrValues.ToDictionary(_ => _.attrPath, _ => _.value))
                            .ToList();

                    values = JoinData(values, primaryKeydata, join.ForeignKeyColumnPath, join.PrimaryKeyColumnPath);
                }

            }

            var selectedValues =
                values.Where(query.Selection.Expression).ToList();

            var projectedValues =
                selectedValues.Select(values => query.Projection.ColumnPaths.ToDictionary(projAttrPath => projAttrPath, projAttrPath => values[projAttrPath]))
                              .ToList();

            var projectedDataTypes =
                attributeDataTypes.Where(attrDataType => query.Projection.ColumnPaths.Contains(attrDataType.Key))
                .ToDictionary(_ => _.Key, _ => _.Value);

            var resultBuilder = TabularDataBuilder.InitTabularData(projectedDataTypes.ToDictionary(kv => kv.Key.Replace("/", "."), kv => kv.Value));

            foreach (var row in projectedValues)
            {
                resultBuilder.AddRow(conf => conf.WithRowData(row.ToDictionary(kv => kv.Key.Replace("/", "."), kv => kv.Value)!));
            }

            return resultBuilder.Build();
        });

    private string AdaptPathToDataSourceLocation(string fullPath)
        => Path.Join(_dataSourceDirectoryPath, fullPath.Split("/", 2).ElementAt(1));

    private List<Dictionary<string, object>> JoinData(List<Dictionary<string, object>> foreignKeyData, List<Dictionary<string, object>> primaryKeyData, string foreignKeyColumnPath, string primaryKeyColumnPath)
    {
        var result = new List<Dictionary<string, object>>();
        foreach (var foreignKeyDataRow in foreignKeyData)
        {
            var foreignKey = foreignKeyDataRow[foreignKeyColumnPath];
            
            var primaryKeyDataRow =
                primaryKeyData.Where(pkDataRow => pkDataRow[primaryKeyColumnPath].Equals(foreignKey))
                              .FirstOrDefault();

            var resultRow = new Dictionary<string, object>();
            foreignKeyDataRow.ToList().ForEach(x => resultRow.Add(x.Key, x.Value));
            primaryKeyDataRow?.ToList().ForEach(x => resultRow.Add(x.Key, x.Value));

            result.Add(resultRow);
        }
        return result;
    }
}
