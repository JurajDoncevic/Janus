﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Wrapper.Core.LocalCommanding;
public abstract class LocalInsert<TInstantiation> : LocalCommand
{
    private readonly TInstantiation _instantiation;

    protected LocalInsert(TInstantiation instantiation, string target) : base(target)
    {
        _instantiation = instantiation;
    }

    public TInstantiation Instantiation => _instantiation;
}
