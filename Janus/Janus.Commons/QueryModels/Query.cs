using FunctionalExtensions.Base.Results;
using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.QueryModels.JsonConversion;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels;

/// <summary>
/// Describes a query
/// </summary>
[JsonConverter(typeof(QueryJsonConverter))]
public class Query
{
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private string _onTableauId;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="onTableuId">Initial query tableau</param>
    /// <param name="projection">Projection clause</param>
    /// <param name="selection">Selection clause</param>
    /// <param name="joining">Joining clause</param>
    internal Query(string onTableuId, Option<Projection> projection, Option<Selection> selection, Option<Joining> joining)
    {
        _onTableauId = onTableuId;
        _projection = projection;
        _selection = selection;
        _joining = joining;
    }

    /// <summary>
    /// Optional projection clause
    /// </summary>
    public Option<Projection> Projection => _projection;

    /// <summary>
    /// Optional selection clause
    /// </summary>
    public Option<Selection> Selection => _selection;

    /// <summary>
    /// Optional joining clause
    /// </summary>
    public Option<Joining> Joining => _joining;

    /// <summary>
    /// Tableau on which the query is initialized
    /// </summary>
    public string OnTableauId { get => _onTableauId; set => _onTableauId = value; }

    public override bool Equals(object? obj)
    {
        return obj is Query query &&
               EqualityComparer<Option<Projection>>.Default.Equals(_projection, query._projection) &&
               EqualityComparer<Option<Selection>>.Default.Equals(_selection, query._selection) &&
               EqualityComparer<Option<Joining>>.Default.Equals(_joining, query._joining) &&
               _onTableauId == query._onTableauId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_projection, _selection, _joining, _onTableauId);
    }

    /// <summary>
    /// Checks validity of query over a data source
    /// </summary>
    /// <param name="dataSource">Data source over which the query should be run</param>
    /// <returns></returns>
    /// <exception cref="InvalidAttributeIdException"></exception>
    /// <exception cref="AttributeDoesNotExistException"></exception>
    /// <exception cref="SelfJoinNotSupportedException"></exception>
    /// <exception cref="JoinedAttributesNotOfSameTypeException"></exception>
    /// <exception cref="PrimaryKeyAttributeNullableException"></exception>
    /// <exception cref="DuplicateJoinNotSupportedException"></exception>
    /// <exception cref="CyclicJoinNotSupportedException"></exception>
    /// <exception cref="TableauPrimaryKeyReferenceNotUniqueException"></exception>
    /// <exception cref="JoinsNotConnectedException"></exception>
    /// <exception cref="DuplicateAttributeAssignedToProjectionException"></exception>
    /// <exception cref="AttributeNotInReferencedTableausException"></exception>
    public Result IsValidForDataSource(DataSource dataSource!!)
    {
        return ResultExtensions.AsResult(() =>
        {
            // check joining validity
            if (_joining)
            {
                IReadOnlyCollection<Join> joins = _joining.Value.Joins;
                foreach (var join in joins)
                {
                    string foreignKeyAttributeId = join.ForeignKeyAttributeId;
                    string primaryKeyAttributeId = join.PrimaryKeyAttributeId;

                    // check if given ids are ok
                    if (!foreignKeyAttributeId.Contains('.'))
                        throw new InvalidAttributeIdException(join.ForeignKeyAttributeId);
                    if (!primaryKeyAttributeId.Contains('.'))
                        throw new InvalidAttributeIdException(join.PrimaryKeyAttributeId);

                    // get tableau ids from the supposed attribute ids
                    var foreignKeyTableauId = join.ForeignKeyAttributeId.Remove(join.ForeignKeyAttributeId.LastIndexOf('.'));
                    var primaryKeyTableauId = join.PrimaryKeyAttributeId.Remove(join.PrimaryKeyAttributeId.LastIndexOf('.'));

                    // check attribute (and tableau) existence
                    if (!dataSource.ContainsAttribute(join.ForeignKeyAttributeId))
                        throw new AttributeDoesNotExistException(join.PrimaryKeyAttributeId, dataSource.Name);
                    if (!dataSource.ContainsAttribute(join.PrimaryKeyAttributeId))
                        throw new AttributeDoesNotExistException(join.PrimaryKeyAttributeId, dataSource.Name);

                    // check for self-join
                    if (foreignKeyTableauId.Equals(primaryKeyTableauId))
                        throw new SelfJoinNotSupportedException(primaryKeyTableauId);

                    // check attribute types - must be the same
                    var fkNames = Utils.GetNamesFromAttributeId(foreignKeyAttributeId);
                    var pkNames = Utils.GetNamesFromAttributeId(primaryKeyAttributeId);
                    var fkAttribute = dataSource[fkNames.schemaName][fkNames.tableauName][fkNames.attributeName];
                    var pkAttribute = dataSource[pkNames.schemaName][pkNames.tableauName][pkNames.attributeName];
                    if (fkAttribute.DataType != pkAttribute.DataType)
                        throw new JoinedAttributesNotOfSameTypeException(foreignKeyAttributeId, fkAttribute.DataType, primaryKeyAttributeId, pkAttribute.DataType);

                    // check pk nullability
                    if (pkAttribute.IsNullable)
                        throw new PrimaryKeyAttributeNullableException(primaryKeyAttributeId);

                    // check for self-join
                    if (foreignKeyTableauId.Equals(primaryKeyTableauId))
                        throw new SelfJoinNotSupportedException(primaryKeyTableauId);
                }
                // check for duplicate joins
                if (joins.Distinct().Count() != joins.Count)
                {
                    var duplicateJoin = joins.GroupBy(j => j)
                                             .Where(g => g.Count() > 1)
                                             .Select(g => g.Key)
                                             .First();
                    throw new DuplicateJoinNotSupportedException(duplicateJoin);
                }
                // check for pk tableaus multiple references
                {
                    var tempJoining = new Joining();
                    foreach (var join in joins)
                    {
                        if (!JoiningUtils.ArePrimaryKeyReferencesUnique(tempJoining, join))
                            throw new TableauPrimaryKeyReferenceNotUniqueException(join);
                        tempJoining.AddJoin(join);
                    }
                }
                // check for cycle joins
                {
                    var tempJoining = new Joining();
                    foreach (var join in joins)
                    {
                        if (JoiningUtils.IsJoiningCyclic(_onTableauId, tempJoining, join))
                            throw new CyclicJoinNotSupportedException(join);
                        tempJoining.AddJoin(join);
                    }
                }

                // check for graph connectedness
                if (!JoiningUtils.IsJoiningConnectedGraph(_onTableauId, _joining.Value))
                    throw new JoinsNotConnectedException();
            }

            HashSet<string> referencedTableauIds = new HashSet<string>();
            referencedTableauIds = _joining.Map(joining => joining.Joins)
                                           .Map(joins => joins.SelectMany(j => new[] { j.ForeignKeyTableauId, j.PrimaryKeyTableauId }))
                                           .Value?.ToHashSet() ?? new HashSet<string>();
            referencedTableauIds.Add(_onTableauId);

            // check projection validity
            if (_projection)
            {
                var projection = _projection.Value;
                // check 
                foreach (var attributeId in projection.IncludedAttributeIds)
                {
                    if (!dataSource.ContainsAttribute(attributeId))
                        throw new AttributeDoesNotExistException(attributeId, dataSource.Name);
                }

                foreach (var attributeId in projection.IncludedAttributeIds)
                {
                    if (projection.IncludedAttributeIds.Count(attrId => attrId.Equals(attributeId)) > 1)
                        throw new DuplicateAttributeAssignedToProjectionException(attributeId);
                }
                foreach (var attributeId in projection.IncludedAttributeIds)
                {
                    if (!dataSource.Schemas
                                   .SelectMany(schema => schema.Tableaus)
                                   .Where(tableau => referencedTableauIds.Contains(tableau.Id))
                                   .Any(tableau => tableau.Attributes.Any(attribute => attribute.Id.Equals(attributeId))))
                        throw new AttributeNotInReferencedTableausException(attributeId);
                }

            }

            // check selection validity
            if (_selection)
            {
                // no checks so far
            }

            return true;
        });

    }
}


