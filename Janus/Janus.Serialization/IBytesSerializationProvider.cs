using Janus.Commons.CommandModels;
using Janus.Commons.Messages;

namespace Janus.Serialization;

/// <summary>
/// Interface for serialization providers with byte format serializers
/// </summary>
public interface IBytesSerializationProvider : ISerializationProvider<byte[]>
{
}
