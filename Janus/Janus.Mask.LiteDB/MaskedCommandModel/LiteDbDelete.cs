﻿using FunctionalExtensions.Base;
using Janus.Commons.SchemaModels;
using Janus.Mask.MaskedCommandModel;

namespace Janus.Mask.LiteDB.MaskedCommandModel;
public class LiteDbDelete : MaskedDelete<Unit>
{
    private LiteDbDelete(TableauId target, Unit selection) : base(target, selection)
    {
    }
}
