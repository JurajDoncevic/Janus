using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Serialization.MongoBson;
internal static class Utils
{
    internal static byte[] ToBson<T>(T data)
    {
        using (var stream = new MemoryStream())
        {
            using (var bsonWriter = new BsonBinaryWriter(stream))
            {
                BsonSerializer.Serialize<T>(bsonWriter, data);
            }
            return stream.ToArray();
        }
    }

    internal static T FromBson<T>(byte[] bytes)
    {
        return BsonSerializer.Deserialize<T>(bytes);
    }
}
