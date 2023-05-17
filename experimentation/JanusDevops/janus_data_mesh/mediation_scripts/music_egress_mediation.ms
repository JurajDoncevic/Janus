SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE MusicData VERSION "1.0"
    WITH SCHEMA Stable
        WITH TABLEAU Tracks
            WITH ATTRIBUTES
                TrackId,
                TrackTitle,
                Composer,
                TrackLengthSeconds,
                AlbumId,
                AlbumTitle,
                ArtistName,
                GenreName
            BEING
                SELECT
                    MusicDomainData.main.Main_Tracks.TrackId,
                    MusicDomainData.main.Main_Tracks.TrackTitle,
                    MusicDomainData.main.Main_Tracks.Composer,
                    MusicDomainData.main.Main_Tracks.TrackLengthSeconds,
                    MusicDomainData.main.Main_Tracks.AlbumId,
                    MusicDomainData.main.Main_Tracks.AlbumTitle,
                    MusicDomainData.main.Main_Tracks.ArtistName,
                    MusicDomainData.main.Main_Tracks.GenreName
                FROM MusicDomainData.main.Main_Tracks
    WITH TABLEAU Albums
        WITH ATTRIBUTES
                AlbumId,
                Title,
                PublishYear,
                ArtistId,
                ArtistName
            BEING
                SELECT
                    MusicDomainData.main.Main_Albums.AlbumId,
                    MusicDomainData.main.Main_Albums.Title,
                    MusicDomainData.main.Main_Albums.PublishYear,
                    MusicDomainData.main.Main_Albums.ArtistId,
                    MusicDomainData.main.Main_Albums.ArtistName
                FROM MusicDomainData.main.Main_Albums
    WITH SCHEMA Fresh
        WITH TABLEAU Tracks
            WITH ATTRIBUTES
                TrackId,
                TrackTitle,
                Composer,
                TrackLengthSeconds,
                AlbumId,
                AlbumTitle,
                ArtistName,
                GenreName
            BEING
                SELECT
                    MusicIngressData.Main.Tracks.TrackId,
                    MusicIngressData.Main.Tracks.TrackTitle,
                    MusicIngressData.Main.Tracks.Composer,
                    MusicIngressData.Main.Tracks.TrackLengthSeconds,
                    MusicIngressData.Main.Tracks.AlbumId,
                    MusicIngressData.Main.Tracks.AlbumTitle,
                    MusicIngressData.Main.Tracks.ArtistName,
                    MusicIngressData.Main.Tracks.GenreName
                FROM MusicIngressData.Main.Tracks
    WITH TABLEAU Albums
        WITH ATTRIBUTES
                AlbumId,
                Title,
                PublishYear,
                ArtistId,
                ArtistName
            BEING
                SELECT
                    MusicIngressData.Main.Albums.AlbumId,
                    MusicIngressData.Main.Albums.Title,
                    MusicIngressData.Main.Albums.PublishYear,
                    MusicIngressData.Main.Albums.ArtistId,
                    MusicIngressData.Main.Albums.ArtistName
                FROM MusicIngressData.Main.Albums