namespace Janus.Commons.SchemaModels.Building;

/// <summary>
/// Helper extension methods to construct components of a schema model from enumerations
/// </summary>
public static class Helpers
{
    /// <summary>
    /// Add schemas to a data source according to the adding function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataSourceEditing"></param>
    /// <param name="values"></param>
    /// <param name="addingFunc"></param>
    /// <returns></returns>
    public static ISchemaAdding AddSchemasWith<T>(this IDataSourceEditing dataSourceEditing, IEnumerable<T> values, Func<T, ISchemaAdding, ISchemaAdding> addingFunc)
    {
        ISchemaAdding schemaAdding = (ISchemaAdding)dataSourceEditing;
        foreach (var item in values)
        {
            schemaAdding = addingFunc(item, schemaAdding);
        }
        return schemaAdding;
    }

    /// <summary>
    /// Add tableaus to a schema according to the adding function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="schemaEditing"></param>
    /// <param name="values"></param>
    /// <param name="addingFunc"></param>
    /// <returns></returns>
    public static ITableauAdding AddTableausWith<T>(this ISchemaEditing schemaEditing, IEnumerable<T> values, Func<T, ITableauAdding, ITableauAdding> addingFunc)
    {
        ITableauAdding tableauAdding = (ITableauAdding)schemaEditing;
        foreach (var item in values)
        {
            tableauAdding = addingFunc(item, tableauAdding);
        }
        return tableauAdding;
    }

    /// <summary>
    /// Add attributes to a tableau according to the adding function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="tableauEditing"></param>
    /// <param name="values"></param>
    /// <param name="addingFunc"></param>
    /// <returns></returns>
    public static IAttributeAdding AddAttributesWith<T>(this ITableauEditing tableauEditing, IEnumerable<T> values, Func<T, IAttributeAdding, IAttributeAdding> addingFunc)
    {
        IAttributeAdding attributeAdding = (IAttributeAdding)tableauEditing;
        foreach (var item in values)
        {
            attributeAdding = addingFunc(item, attributeAdding);
        }
        return attributeAdding;
    }

    /// <summary>
    /// Add update sets according to the adding function
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="attributeAdding"></param>
    /// <param name="values"></param>
    /// <param name="addingFunc"></param>
    /// <returns></returns>
    public static IUpdateSetAdding AddUpdateSetsWith<T>(this IAttributeAdding attributeAdding, IEnumerable<T> values, Func<T, IUpdateSetAdding, IUpdateSetAdding> addingFunc)
    {
        IUpdateSetAdding updateSetAdding = (IUpdateSetAdding)attributeAdding;
        foreach (var item in values)
        {
            updateSetAdding = addingFunc(item, updateSetAdding);
        }
        return updateSetAdding;
    }
}
