namespace Janus.Commons;

/// <summary>
/// Officially recognized communication formats
/// </summary>
public enum CommunicationFormats
{
    /// <summary>
    /// Apache Avro (binary)
    /// </summary>
    AVRO,
    /// <summary>
    /// Binary Javascript Object Notation (binary)
    /// </summary>
    BSON,
    /// <summary>
    /// MongoDB's Binary Javascript Object Notation (binary)
    /// </summary>
    MONGO_BSON,
    /// <summary>
    /// ProtocolBuffers (binary)
    /// </summary>
    PROTOBUFS,
    /// <summary>
    /// Javascript Object Notation (textual)
    /// </summary>
    JSON,
    /// <summary>
    /// Unknown data format - something went bad :(
    /// </summary>
    UNKNOWN
}
