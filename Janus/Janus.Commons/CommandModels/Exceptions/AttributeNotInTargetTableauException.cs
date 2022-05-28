﻿using System.Runtime.Serialization;

namespace Janus.Commons.CommandModels.Exceptions;

public class AttributeNotInTargetTableauException : Exception
{
    public AttributeNotInTargetTableauException(string invalidAttributeId, string tableauId) 
        : base($"Invalid attribute reference {invalidAttributeId} for tableau {tableauId}")
    {
    }
}