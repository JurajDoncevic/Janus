SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE ListeningIngressData VERSION "1.0"
    WITH SCHEMA Main
        WITH TABLEAU ListeningHistory
            WITH ATTRIBUTES
                ListenId,
                CustomerUserName,
                ListenedTrackId,
                ListenedTrackName,
                ListenedOn,
                ListenedToEnd,
                StoppedAtSeconds,
                ListenedMediaType
            BEING
                SELECT
                    ListeningData.main.listening_history.listen_id,
                    CustomersData.Stable.Customers.UserName,
                    ListeningData.main.listening_history.listened_track_id,
                    MusicData.Stable.Tracks.TrackTitle,
                    ListeningData.main.listening_history.listened_on,
                    ListeningData.main.listening_history.listened_to_end,
                    ListeningData.main.listening_history.stopped_at_seconds,
                    TracksInstancesData.main.media_types.name
                FROM ListeningData.main.listening_history
                JOIN CustomersData.Stable.Customers
                    ON ListeningData.main.listening_history.customer_id == CustomersData.Stable.Customers.CustomerId
                JOIN MusicData.Stable.Tracks
                    ON ListeningData.main.listening_history.listened_track_id == MusicData.Stable.Tracks.TrackId
                JOIN TracksInstancesData.main.media_types
                    ON ListeningData.main.listening_history.listened_media_type_id == TracksInstancesData.main.media_types.media_type_id
