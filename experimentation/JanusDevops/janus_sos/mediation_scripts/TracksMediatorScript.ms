SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE TracksData VERSION "1.0"
    WITH SCHEMA Main 
        WITH TABLEAU Genres
            WITH ATTRIBUTES
                GenreId,
                Name
            BEING
                SELECT
                    TracksData.main.genres.GenreId,
                    TracksData.main.genres.Name
                FROM TracksData.main.genres
        WITH TABLEAU MediaTypes
            WITH ATTRIBUTES
                MediaTypeId,
                Name
            BEING
                SELECT
                    TracksData.main.media_types.MediaTypeId,
                    TracksData.main.media_types.Name
                FROM TracksData.main.media_types
        WITH TABLEAU Tracks
            WITH ATTRIBUTES
                TrackId,
                Name,
                AlbumId,
                MediaTypeId,
                GenreId,
                Composer,
                Milliseconds,
                Bytes,
                UnitPrice
            BEING
                SELECT
                    TracksData.main.tracks.TrackId,
                    TracksData.main.tracks.Name,
                    TracksData.main.tracks.AlbumId,
                    TracksData.main.tracks.MediaTypeId,
                    TracksData.main.tracks.GenreId,
                    TracksData.main.tracks.Composer,
                    TracksData.main.tracks.Milliseconds,
                    TracksData.main.tracks.Bytes,
                    TracksData.main.tracks.UnitPrice
                FROM TracksData.main.tracks

