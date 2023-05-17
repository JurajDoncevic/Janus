SETTING
    PROPAGATE ATTRIBUTE DESCRIPTIONS
DATASOURCE SubscriptionsIngressData VERSION "1.0"
    WITH SCHEMA Main
        WITH TABLEAU SingleUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    CustomersPlansData.main.single_plans_customers.plan_id,
                    CustomersData.Stable.Customers.UserName,
                    CustomersData.Stable.Customers.Email,
                    CustomersPlansData.main.customers_plans.started_on,
                    CustomersPlansData.main.customers_plans.ended_on
                FROM CustomersPlansData.main.single_plans_customers
                JOIN CustomersPlansData.main.customers_plans
                    ON CustomersPlansData.main.single_plans_customers.plan_id == CustomersPlansData.main.customers_plans.plan_id
                JOIN CustomersData.Stable.Customers
                    ON CustomersPlansData.main.customers_plans.billing_customer_id == CustomersData.Stable.Customers.CustomerId
        WITH TABLEAU DualUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    CustomersPlansData.main.dual_plans_customers.plan_id,
                    CustomersData.Stable.Customers.UserName,
                    CustomersData.Stable.Customers.Email,
                    CustomersPlansData.main.customers_plans.started_on,
                    CustomersPlansData.main.customers_plans.ended_on
                FROM CustomersPlansData.main.dual_plans_customers
                JOIN CustomersPlansData.main.customers_plans
                    ON CustomersPlansData.main.dual_plans_customers.plan_id == CustomersPlansData.main.customers_plans.plan_id
                JOIN CustomersData.Stable.Customers
                    ON CustomersPlansData.main.customers_plans.billing_customer_id == CustomersData.Stable.Customers.CustomerId
        WITH TABLEAU MultiUserSubscriptions
            WITH ATTRIBUTES
                PlanId,
                BillingUserName,
                BillingUserEmail,
                StartedOn,
                EndedOn
            BEING
                SELECT
                    CustomersPlansData.main.multi_plans_customers.plan_id,
                    CustomersData.Stable.Customers.UserName,
                    CustomersData.Stable.Customers.Email,
                    CustomersPlansData.main.customers_plans.started_on,
                    CustomersPlansData.main.customers_plans.ended_on
                FROM CustomersPlansData.main.multi_plans_customers
                JOIN CustomersPlansData.main.customers_plans
                    ON CustomersPlansData.main.multi_plans_customers.plan_id == CustomersPlansData.main.customers_plans.plan_id
                JOIN CustomersData.Stable.Customers
                    ON CustomersPlansData.main.customers_plans.billing_customer_id == CustomersData.Stable.Customers.CustomerId                    