﻿using Janus.Communication.Nodes.Implementations;
using Janus.Communication.Tests.Mocks;
using Microsoft.Extensions.Configuration;
using Janus.Commons.SchemaModels;
using Janus.Commons.QueryModels;
using Janus.Commons.DataModels;
using Janus.Commons.CommandModels;
using static Janus.Commons.SelectionExpressions.Expressions;
using Janus.Serialization;
using Janus.Serialization.Avro;

namespace Janus.Communication.Tests.TestFixtures;

public class BsonCommunicationNodeTestFixture : CommunicationNodeTestFixture
{
    public override IBytesSerializationProvider SerializationProvider => new AvroSerializationProvider();
}
