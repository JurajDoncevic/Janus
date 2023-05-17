SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE MusicIngressData VERSION "1.0"
    WITH SCHEMA Main
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
                    MusicData.main.tracks.track_id,
                    MusicData.main.tracks.title,
                    MusicData.main.tracks.composer,
                    MusicData.main.tracks.track_length_seconds,
                    MusicData.main.tracks.album_id,
                    MusicData.main.albums.title,
                    MusicData.main.artists.name,
                    MusicData.main.genres.name
                FROM MusicData.main.tracks
                JOIN MusicData.main.albums
                    ON MusicData.main.tracks.album_id == MusicData.main.albums.album_id
                JOIN MusicData.main.artists
                    ON MusicData.main.albums.artist_id == MusicData.main.artists.artist_id
                JOIN MusicData.main.genres
                    ON MusicData.main.tracks.genre_id == MusicData.main.genres.genre_id
    WITH TABLEAU Albums
        WITH ATTRIBUTES
                AlbumId,
                Title,
                PublishYear,
                ArtistId,
                ArtistName
            BEING
                SELECT
                    MusicData.main.albums.album_id,
                    MusicData.main.albums.title,
                    MusicData.main.albums.publish_year,
                    MusicData.main.artists.artist_id,
                    MusicData.main.artists.name
                FROM MusicData.main.albums
                JOIN MusicData.main.artists
                    ON MusicData.main.albums.artist_id == MusicData.main.artists.artist_id