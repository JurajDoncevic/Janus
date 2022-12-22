using Janus.Commons.SchemaModels;
using Janus.Communication.Remotes;

namespace Janus.Components.IntegrationTests;
public class MediationUseCaseTests
{
    private const string _testAddress = "127.0.0.1";

    [Theory]
    [InlineData(
        @"DATASOURCE test_med
          WITH SCHEMA main
          WITH TABLEAU tracks
          WITH ATTRIBUTES 
          id_tracks, track_name, duration_ms, album_name, artist_name
          BEING
          SELECT test1.main.tracks.TrackId, test1.main.tracks.Name, test1.main.tracks.Milliseconds, test2.main.albums.Title, test2.main.artists.Name
          FROM test1.main.tracks
          JOIN test2.main.albums ON test1.main.tracks.AlbumId == test2.main.albums.AlbumId
          JOIN test2.main.artists ON test2.main.albums.ArtistId == test2.main.artists.ArtistId;",
        "SELECT * FROM test_med.main.tracks;"
        )]
    [InlineData(
        @"DATASOURCE test_med VERSION ""1.0""
          WITH SCHEMA main
          WITH TABLEAU albums
          WITH ATTRIBUTES 
          album_id, album_title, artist_name
          BEING
          SELECT test1.main.albums.AlbumId, test1.main.albums.Title, test1.main.artists.Name
          FROM test1.main.albums
          JOIN test1.main.artists ON test1.main.albums.ArtistId == test1.main.artists.ArtistId;",
        @"SELECT test_med.main.albums.album_id, test_med.main.albums.album_title, test_med.main.albums.artist_name 
          FROM test_med.main.albums
          WHERE test_med.main.albums.artist_name == ""AC/DC"" OR test_med.main.albums.artist_name == ""Queen"";"
        )]
    [InlineData(
        @"DATASOURCE test_med
          WITH SCHEMA main
          WITH TABLEAU tracks
          WITH ATTRIBUTES 
          id_tracks, track_name, duration_ms, album_name, artist_name
          BEING
          SELECT test1.main.tracks.TrackId, test1.main.tracks.Name, test1.main.tracks.Milliseconds, test2.main.albums.Title, test2.main.artists.Name
          FROM test1.main.tracks
          JOIN test2.main.albums ON test1.main.tracks.AlbumId == test2.main.albums.AlbumId
          JOIN test2.main.artists ON test2.main.albums.ArtistId == test2.main.artists.ArtistId;",
        @"SELECT * 
          FROM test_med.main.tracks
          WHERE test_med.main.tracks.id_tracks == 1;"
        )]
    [InlineData(
        @"DATASOURCE test_med
          WITH SCHEMA main
          WITH TABLEAU tracks
          WITH ATTRIBUTES 
          id_tracks, track_name, duration_ms, album_name, artist_name
          BEING
          SELECT test1.main.tracks.TrackId, test1.main.tracks.Name, test1.main.tracks.Milliseconds, test2.main.albums.Title, test2.main.artists.Name
          FROM test1.main.tracks
          JOIN test2.main.albums ON test1.main.tracks.AlbumId == test2.main.albums.AlbumId
          JOIN test2.main.artists ON test2.main.albums.ArtistId == test2.main.artists.ArtistId;",
        @"SELECT * 
          FROM test_med.main.tracks
          WHERE test_med.main.tracks.duration_ms > 300000 AND test_med.main.tracks.artist_name == ""Queen"";"
        )]
    public async Task MediateTwoWrappersAsync(string mediationScript, string mediatedQueryText)
    {
        var mediator1Options = ComponentOptions.MediatorOptions.ElementAt(0);
        var (mediator1HostId, mediator1Manager) = ComponentInstances.CreateMediatorHostedInstance(mediator1Options);
        var mediator1RemotePoint = new UndeterminedRemotePoint(_testAddress, mediator1Options.ListenPort);

        var wrapper1Options = ComponentOptions.WrapperOptions.ElementAt(0);
        var (wrapper1HostId, wrapper1Manager) = ComponentInstances.CreateSqliteWrapperHostedInstance(wrapper1Options);
        var wrapper1RemotePoint = new UndeterminedRemotePoint(_testAddress, wrapper1Options.ListenPort);

        var wrapper2Options = ComponentOptions.WrapperOptions.ElementAt(1);
        var (wrapper2HostId, wrapper2Manager) = ComponentInstances.CreateSqliteWrapperHostedInstance(wrapper2Options);
        var wrapper2RemotePoint = new UndeterminedRemotePoint(_testAddress, wrapper2Options.ListenPort);

        await mediator1Manager.RegisterRemotePoint(wrapper1RemotePoint);
        await mediator1Manager.RegisterRemotePoint(wrapper2RemotePoint);
        await wrapper1Manager.GenerateSchema();
        await wrapper2Manager.GenerateSchema();

        var schemaLoading =
            await Task.WhenAll(
                mediator1Manager.GetRegisteredRemotePoints()
                .Map(rp => mediator1Manager.LoadSchemaFrom(rp))
            );
                     

        var mediationResult = 
                await Task.FromResult(mediator1Manager.CreateDataSourceMediation(mediationScript))
                    .Bind(dataSourceMediation => mediator1Manager.ApplyMediation(dataSourceMediation));

        var queryResult = 
            await mediator1Manager.CreateQuery(mediatedQueryText)
                .Bind(query => mediator1Manager.RunQuery(query));



        ComponentInstances.DisposeOfComponent(mediator1HostId);
        ComponentInstances.DisposeOfComponent(wrapper1HostId);
        ComponentInstances.DisposeOfComponent(wrapper2HostId);

        Assert.True(mediationResult);
        Assert.True(queryResult);
    }
}
