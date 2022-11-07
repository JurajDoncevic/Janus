﻿namespace Janus.Mediation.MediationModels.Exceptions;

public class InvalidAttributeIdException : Exception
{
    public InvalidAttributeIdException(string attributeId)
        : base($"Invalid attribute id give:{attributeId}. Must contain at least one '.'")
    {
    }
}
