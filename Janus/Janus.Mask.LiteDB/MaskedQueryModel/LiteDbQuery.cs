﻿using Janus.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedQueryModel;

namespace Janus.Mask.LiteDB.MaskedQueryModel;
public sealed class LiteDbQuery : MaskedQuery<TableauId, Unit, Unit, Unit>
{
    private LiteDbQuery(TableauId startingWith, Unit selection, Unit joining, Unit projection) : base(startingWith, selection, joining, projection)
    {
    }
}
