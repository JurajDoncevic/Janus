SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE PlaylistsUsersData VERSION "1.0"
    WITH SCHEMA Main
        WITH TABLEAU Customers
            WITH ATTRIBUTES
                CustomerId,
                FirstName,
                LastName,
                Company,
                Address,
                City,
                State,
                Country,
                PostalCode,
                Phone,
                Fax,
                Email,
                SupportRepId
            BEING
                SELECT
                    PlaylistsUsersData.main.customers.CustomerId,
                    PlaylistsUsersData.main.customers.FirstName,
                    PlaylistsUsersData.main.customers.LastName,
                    PlaylistsUsersData.main.customers.Company,
                    PlaylistsUsersData.main.customers.Address,
                    PlaylistsUsersData.main.customers.City,
                    PlaylistsUsersData.main.customers.State,
                    PlaylistsUsersData.main.customers.Country,
                    PlaylistsUsersData.main.customers.PostalCode,
                    PlaylistsUsersData.main.customers.Phone,
                    PlaylistsUsersData.main.customers.Fax,
                    PlaylistsUsersData.main.customers.Email,
                    PlaylistsUsersData.main.customers.SupportRepId
                FROM PlaylistsUsersData.main.customers
        WITH TABLEAU Playlists
            WITH ATTRIBUTES
                PlaylistId,
                Name,
                CreatorId
            BEING
                SELECT
                    PlaylistsUsersData.main.playlists.PlaylistId,
                    PlaylistsUsersData.main.playlists.Name,
                    PlaylistsUsersData.main.playlists.CreatorId
                FROM PlaylistsUsersData.main.playlists
        WITH TABLEAU PlaylistTrack
            WITH ATTRIBUTES
                PlaylistId,
                TrackId
            BEING
                SELECT
                    PlaylistsUsersData.main.playlist_track.PlaylistId,
                    PlaylistsUsersData.main.playlist_track.TrackId
                FROM PlaylistsUsersData.main.playlist_track

