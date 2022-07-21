using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// Javascript Object Notation (textual)
    /// </summary>
    JSON
}
