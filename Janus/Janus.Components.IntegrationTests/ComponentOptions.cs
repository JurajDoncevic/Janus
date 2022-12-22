using Janus.Communication.Remotes;
using Janus.Mediator;
using Janus.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Components.IntegrationTests;
internal static class ComponentOptions
{
    internal static IEnumerable<WrapperOptions> WrapperOptions => new List<WrapperOptions>
    {
        new WrapperOptions(
            "TEST_sqlite_wrapper_1",
            10001,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "Data Source = ./test1.db",
            true,
            "sqlite_wrapper1_database.db",
            "test1"
            ),
        new WrapperOptions(
            "TEST_sqlite_wrapper_2",
            10002,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "Data Source = ./test2.db",
            true,
            "sqlite_wrapper2_database.db",
            "test2"
            ),
        new WrapperOptions(
            "TEST_sqlite_wrapper_3",
            10003,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "Data Source = ./test3.db",
            true,
            "sqlite_wrapper3_database.db",
            "test3"
            ),
        new WrapperOptions(
            "TEST_sqlite_wrapper_4",
            10004,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "Data Source = ./test4.db",
            true,
            "sqlite_wrapper4_database.db",
            "test4"
            ),
    };

    internal static IEnumerable<MediatorOptions> MediatorOptions => new List<MediatorOptions>
    {
        new MediatorOptions(
            "TEST_Mediator1",
            20001,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "mediator1_database.db"
            ),
        new MediatorOptions(
            "TEST_Mediator2",
            20002,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "mediator2_database.db"
            ),
        new MediatorOptions(
            "TEST_Mediator3",
            20003,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "mediator3_database.db"
            ),
        new MediatorOptions(
            "TEST_Mediator4",
            20004,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "mediator4_database.db"
            ),
        new MediatorOptions(
            "TEST_Mediator5",
            20005,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            "mediator5_database.db"
            ),
    };
}
