SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE AlbumsArtistsData VERSION "1.0"
    WITH SCHEMA Main 
        WITH TABLEAU Albums
            WITH ATTRIBUTES
                AlbumId,
                Title,
                ArtistId
            BEING
                SELECT
                    AlbumsArtistsData.main.albums.AlbumId,
                    AlbumsArtistsData.main.albums.Title,
                    AlbumsArtistsData.main.albums.ArtistId
                FROM AlbumsArtistsData.main.albums
        WITH TABLEAU Artists
            WITH ATTRIBUTES
                ArtistId,
                Name
            BEING
                SELECT
                    AlbumsArtistsData.main.artists.ArtistId,
                    AlbumsArtistsData.main.artists.Name
                FROM AlbumsArtistsData.main.artists
