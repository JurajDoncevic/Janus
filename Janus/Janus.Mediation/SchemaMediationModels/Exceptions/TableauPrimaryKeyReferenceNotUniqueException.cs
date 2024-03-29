﻿using Janus.Mediation.SchemaMediationModels.MediationQueryModels;

namespace Janus.Mediation.SchemaMediationModels.Exceptions;

public class TableauPrimaryKeyReferenceNotUniqueException : Exception
{
    public TableauPrimaryKeyReferenceNotUniqueException(Join join)
        : base($"Tableau {join.PrimaryKeyTableauId} is re-referenced as a primary key table in a new join: {join.ForeignKeyAttributeId}-{join.PrimaryKeyAttributeId}")
    {
    }
}
