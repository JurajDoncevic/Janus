SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE SubscriptionsData VERSION "1.0"
    WITH SCHEMA Stable
        WITH TABLEAU SingleUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    SubscriptionsDomainData.main.Main_SingleUserSubscriptions.PlanId,
                    SubscriptionsDomainData.main.Main_SingleUserSubscriptions.BillingUserName,
                    SubscriptionsDomainData.main.Main_SingleUserSubscriptions.BillingUserEmail,
                    SubscriptionsDomainData.main.Main_SingleUserSubscriptions.StartedOn,
                    SubscriptionsDomainData.main.Main_SingleUserSubscriptions.EndedOn
                FROM SubscriptionsDomainData.main.Main_SingleUserSubscriptions
        WITH TABLEAU DualUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    SubscriptionsDomainData.main.Main_DualUserSubscriptions.PlanId,
                    SubscriptionsDomainData.main.Main_DualUserSubscriptions.BillingUserName,
                    SubscriptionsDomainData.main.Main_DualUserSubscriptions.BillingUserEmail,
                    SubscriptionsDomainData.main.Main_DualUserSubscriptions.StartedOn,
                    SubscriptionsDomainData.main.Main_DualUserSubscriptions.EndedOn
                FROM SubscriptionsDomainData.main.Main_DualUserSubscriptions
        WITH TABLEAU MultiUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    SubscriptionsDomainData.main.Main_MultiUserSubscriptions.PlanId,
                    SubscriptionsDomainData.main.Main_MultiUserSubscriptions.BillingUserName,
                    SubscriptionsDomainData.main.Main_MultiUserSubscriptions.BillingUserEmail,
                    SubscriptionsDomainData.main.Main_MultiUserSubscriptions.StartedOn,
                    SubscriptionsDomainData.main.Main_MultiUserSubscriptions.EndedOn
                FROM SubscriptionsDomainData.main.Main_MultiUserSubscriptions
    WITH SCHEMA Fresh
        WITH TABLEAU SingleUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    SubscriptionsIngressData.Main.SingleUserSubscriptions.PlanId,
                    SubscriptionsIngressData.Main.SingleUserSubscriptions.BillingUserName,
                    SubscriptionsIngressData.Main.SingleUserSubscriptions.BillingUserEmail,
                    SubscriptionsIngressData.Main.SingleUserSubscriptions.StartedOn,
                    SubscriptionsIngressData.Main.SingleUserSubscriptions.EndedOn
                FROM SubscriptionsIngressData.Main.SingleUserSubscriptions
        WITH TABLEAU DualUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    SubscriptionsIngressData.Main.DualUserSubscriptions.PlanId,
                    SubscriptionsIngressData.Main.DualUserSubscriptions.BillingUserName,
                    SubscriptionsIngressData.Main.DualUserSubscriptions.BillingUserEmail,
                    SubscriptionsIngressData.Main.DualUserSubscriptions.StartedOn,
                    SubscriptionsIngressData.Main.DualUserSubscriptions.EndedOn
                FROM SubscriptionsIngressData.Main.DualUserSubscriptions
        WITH TABLEAU MultiUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    SubscriptionsIngressData.Main.MultiUserSubscriptions.PlanId,
                    SubscriptionsIngressData.Main.MultiUserSubscriptions.BillingUserName,
                    SubscriptionsIngressData.Main.MultiUserSubscriptions.BillingUserEmail,
                    SubscriptionsIngressData.Main.MultiUserSubscriptions.StartedOn,
                    SubscriptionsIngressData.Main.MultiUserSubscriptions.EndedOn
                FROM SubscriptionsIngressData.Main.MultiUserSubscriptions