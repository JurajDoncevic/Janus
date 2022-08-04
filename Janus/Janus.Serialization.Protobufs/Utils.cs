using ProtoBuf;

namespace Janus.Serialization.Protobufs;
internal static class Utils
{
    internal static byte[] ToProtobufs<T>(T data)
    {
        using (var stream = new MemoryStream())
        {

            Serializer.Serialize(stream, data);
            return stream.ToArray();
        }
    }

    internal static T FromProtobufs<T>(byte[] bytes)
    {
        using (var stream = new MemoryStream(bytes))
        {
            return Serializer.Deserialize<T>(stream);
        }
    }
}
