SETTING
    PROPAGATE UPDATE SETS
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE MusicData VERSION "1.0" #Mediated data about music#
    WITH SCHEMA Main #Default schema#
        WITH TABLEAU Albums #Data about albums#
            WITH ATTRIBUTES
                AlbumId,
                AlbumTitle,
                ArtistName
            BEING
                SELECT 
                    AlbumsArtistsData.main.albums.AlbumId, 
                    AlbumsArtistsData.main.albums.Title, 
                    AlbumsArtistsData.main.artists.Name
                FROM AlbumsArtistsData.main.albums
                JOIN AlbumsArtistsData.main.artists
                    ON AlbumsArtistsData.main.albums.ArtistId == AlbumsArtistsData.main.artists.ArtistId
        WITH TABLEAU Tracks #Data about tracks and albums#
            WITH ATTRIBUTES
                TrackId,
                TrackName,
                GenreName,
                MediaType,
                AlbumTitle,
                DurationMs,
                Composer
            BEING
                SELECT 
                    TracksData.main.tracks.TrackId, 
                    TracksData.main.tracks.Name, 
                    TracksData.main.genres.Name, 
                    TracksData.main.media_types.Name, 
                    AlbumsArtistsData.main.albums.Title, 
                    TracksData.main.tracks.Milliseconds, 
                    TracksData.main.tracks.Composer
                FROM TracksData.main.tracks
                JOIN TracksData.main.genres
                    ON TracksData.main.tracks.GenreId == TracksData.main.genres.GenreId
                JOIN TracksData.main.media_types
                    ON TracksData.main.tracks.MediaTypeId == TracksData.main.media_types.MediaTypeId
                JOIN AlbumsArtistsData.main.albums
                    ON TracksData.main.tracks.AlbumId == AlbumsArtistsData.main.albums.AlbumId
        WITH TABLEAU Users #Data about users/customers#
            WITH ATTRIBUTES
                UserId,
                UserEmail,
                UserFirstName,
                UserLastName,
                UserCountry
            SELECT
                PlaylistsUsersData.main.customers.CustomerId,
                PlaylistsUsersData.main.customers.Email,
                PlaylistsUsersData.main.customers.FirstName,
                PlaylistsUsersData.main.customers.LastName,
                PlaylistsUsersData.main.customers.Country
            FROM PlaylistsUsersData.main.customers
        WITH TABLEAU Playlists #Data about playlists#
            WITH ATTRIBUTES
                PlaylistId,
                PlaylistName,
                CreatorEmail
            BEING
                SELECT
                    PlaylistsUsersData.main.playlists.PlaylistId,
                    PlaylistsUsersData.main.playlists.Name,
                    PlaylistsUsersData.main.customers.Email
                FROM PlaylistsUsersData.main.playlists
                JOIN PlaylistsUsersData.main.customers
                    ON PlaylistsUsersData.main.playlists.CreatorId == PlaylistsUsersData.main.customers.CustomerId
        WITH TABLEAU PlaylistTracks #Tracks in playlists#
            WITH ATTRIBUTES
                TrackId,
                TrackName,
                TrackGenre,
                PlaylistId,
                PlaylistName
            BEING
                SELECT
                    PlaylistsUsersData.main.playlist_track.TrackId,
                    TracksData.main.tracks.Name,
                    TracksData.main.genres.Name,
                    PlaylistsUsersData.main.playlist_track.PlaylistId,
                    PlaylistsUsersData.main.playlists.Name
                FROM PlaylistsUsersData.main.playlist_track
                JOIN TracksData.main.tracks
                    ON PlaylistsUsersData.main.playlist_track.TrackId == TracksData.main.tracks.TrackId
                JOIN PlaylistsUsersData.main.playlists 
                    ON PlaylistsUsersData.main.playlist_track.PlaylistId == PlaylistsUsersData.main.playlists.PlaylistId
                JOIN TracksData.main.genres
                    ON TracksData.main.tracks.GenreId == TracksData.main.genres.GenreId



