using Janus.Commons.SchemaModels;
using ProtoBuf;

namespace Janus.Serialization.Protobufs.SchemaModels.DTOs;

[ProtoContract]
internal sealed class AttributeDto
{
    [ProtoMember(1)]
    public string Name { get; set; }

    [ProtoMember(2)]
    public DataTypes DataType { get; set; }

    [ProtoMember(3)]
    public bool IsIdentity { get; set; }

    [ProtoMember(4)]
    public bool IsNullable { get; set; }

    [ProtoMember(5)]
    public int Ordinal { get; set; }

    [ProtoMember(6)]
    public string Description { get; set; }
}
