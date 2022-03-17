using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Commons.Communication.Messages;

/// <summary>
/// Interface for all system messages
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Preamble or message type. Each message type has a specific name
    /// </summary>
    public string Preamble { get; }
    /// <summary>
    /// Serialize message to binary JSON
    /// </summary>
    /// <returns></returns>
    byte[] ToBson();
}
