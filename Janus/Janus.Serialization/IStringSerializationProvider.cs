using Janus.Commons.CommandModels;
using Janus.Commons.Messages;

namespace Janus.Serialization;

/// <summary>
/// Interface for serialization providers with string format serializers
/// </summary>
public interface IStringSerializationProvider : ISerializationProvider<string>
{
}
