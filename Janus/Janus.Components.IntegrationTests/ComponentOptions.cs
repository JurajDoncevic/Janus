using Janus.Communication.Remotes;
using Janus.Mediator;
using Janus.Wrapper;
using Janus.Wrapper.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Janus.Components.IntegrationTests;
internal static class ComponentOptions
{
    internal static IEnumerable<SqliteWrapperOptions> WrapperOptions => new List<SqliteWrapperOptions>
    {
        new SqliteWrapperOptions(
            "TEST_sqlite_wrapper_1",
            10001,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            false,
            "Data Source = ./test1.db",
            true,
            "sqlite_wrapper1_database.db",
            "test1"
            ),
        new SqliteWrapperOptions(
            "TEST_sqlite_wrapper_2",
            10002,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            false,
            "Data Source = ./test2.db",
            true,
            "sqlite_wrapper2_database.db",
            "test2"
            ),
        new SqliteWrapperOptions(
            "TEST_sqlite_wrapper_3",
            10003,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            false,
            "Data Source = ./test3.db",
            true,
            "sqlite_wrapper3_database.db",
            "test3"
            ),
        new SqliteWrapperOptions(
            "TEST_sqlite_wrapper_4",
            10004,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            false,
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
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            Enumerable.Empty<string>(),
            string.Empty,
            "mediator1_database.db"
            ),
        new MediatorOptions(
            "TEST_Mediator2",
            20002,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            Enumerable.Empty<string>(),
            string.Empty,
            "mediator2_database.db"
            ),
        new MediatorOptions(
            "TEST_Mediator3",
            20003,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            Enumerable.Empty<string>(),
            string.Empty,
            "mediator3_database.db"
            ),
        new MediatorOptions(
            "TEST_Mediator4",
            20004,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            Enumerable.Empty<string>(),
            string.Empty,
            "mediator4_database.db"
            ),
        new MediatorOptions(
            "TEST_Mediator5",
            20005,
            5000,
            Commons.CommunicationFormats.AVRO,
            Commons.NetworkAdapterTypes.TCP,
            false,
            Enumerable.Empty<UndeterminedRemotePoint>(),
            Enumerable.Empty<string>(),
            string.Empty,
            "mediator5_database.db"
            ),
    };
}
