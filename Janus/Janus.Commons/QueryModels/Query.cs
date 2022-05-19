﻿using Janus.Commons.QueryModels.Exceptions;
using Janus.Commons.QueryModels.JsonConversion;
using Janus.Commons.SchemaModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Janus.Commons.QueryModels;

[JsonConverter(typeof(QueryJsonConverter))]
public class Query
{
    private Option<Projection> _projection;
    private Option<Selection> _selection;
    private Option<Joining> _joining;
    private string _onTableauId;

    public Query(string OnTableuId, Option<Projection> projection, Option<Selection> selection, Option<Joining> joining)
    {
        _onTableauId = OnTableuId;
        _projection = projection;
        _selection = selection;
        _joining = joining;
    }

    public Option<Projection> Projection => _projection;

    public Option<Selection> Selection => _selection;

    public Option<Joining> Joining => _joining;

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

    public bool IsValidForDataSource(DataSource dataSource!!)
    {
        // check joining validity
        if (_joining.IsSome)
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
            // check for cycle joins
            {
                var tempJoining = new Joining();
                foreach (var join in joins)
                {
                    if (JoiningUtils.IsJoiningCyclic(tempJoining, join))
                        throw new CyclicJoinNotSupportedException(join);
                    tempJoining.AddJoin(join);
                }
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

            // check for graph connectedness
            if (!JoiningUtils.IsJoiningConnectedGraph(_joining.Value))
                throw new JoinsNotConnectedException();
        }

        HashSet<string> referencedTableauIds = new HashSet<string>(); 
        referencedTableauIds = _joining.Map(joining => joining.Joins)
                                       .Map(joins => joins.SelectMany(j => new[] {j.ForeignKeyTableauId, j.PrimaryKeyTableauId}))
                                       .Value?.ToHashSet() ?? new HashSet<string>();
        referencedTableauIds.Add(_onTableauId);

        // check projection validity
        if (_projection.IsSome)
        {
            var projection = _projection.Value;
            foreach (var attributeId in projection.IncludedAttributeIds)
            {
                if (!dataSource.ContainsAttribute(attributeId))
                    throw new AttributeDoesNotExistException(attributeId, dataSource.Name);
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
        if (_selection.IsSome)
        {
            // no checks so far
        }

        return true;
    }
}


