﻿using Janus.Serialization;
using Janus.Serialization.Avro;

namespace Janus.Communication.Tests.TestFixtures;

public class AvroCommunicationNodeTestFixture : CommunicationNodeTestFixture
{
    public override IBytesSerializationProvider SerializationProvider => new AvroSerializationProvider();
}
