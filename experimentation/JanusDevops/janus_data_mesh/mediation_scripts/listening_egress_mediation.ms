SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE ListeningData VERSION "1.0"
    WITH SCHEMA Stable
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
                    ListeningDomainData.main.Main_ListeningHistory.ListenId,
                    ListeningDomainData.main.Main_ListeningHistory.CustomerUserName,
                    ListeningDomainData.main.Main_ListeningHistory.ListenedTrackId,
                    ListeningDomainData.main.Main_ListeningHistory.ListenedTrackName,
                    ListeningDomainData.main.Main_ListeningHistory.ListenedOn,
                    ListeningDomainData.main.Main_ListeningHistory.ListenedToEnd,
                    ListeningDomainData.main.Main_ListeningHistory.StoppedAtSeconds,
                    ListeningDomainData.main.Main_ListeningHistory.ListenedMediaType
                FROM ListeningDomainData.main.Main_ListeningHistory
    WITH SCHEMA Fresh
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
                    ListeningIngressData.Main.ListeningHistory.ListenId,
                    ListeningIngressData.Main.ListeningHistory.CustomerUserName,
                    ListeningIngressData.Main.ListeningHistory.ListenedTrackId,
                    ListeningIngressData.Main.ListeningHistory.ListenedTrackName,
                    ListeningIngressData.Main.ListeningHistory.ListenedOn,
                    ListeningIngressData.Main.ListeningHistory.ListenedToEnd,
                    ListeningIngressData.Main.ListeningHistory.StoppedAtSeconds,
                    ListeningIngressData.Main.ListeningHistory.ListenedMediaType
                FROM ListeningIngressData.Main.ListeningHistory        

